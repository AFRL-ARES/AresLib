using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Analyzing;
using Ares.Core.AresEnvironment;
using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Executors.Composers;
using Ares.Core.Execution.Extensions;
using Ares.Core.Execution.StopConditions;
using Ares.Core.Planning;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

public class CampaignExecutor : ICampaignExecutor
{
  private readonly IExecutionReporter _executionReporter;
  private readonly ISubject<CampaignExecutionStatus> _executionStatusSubject;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly ICommandComposer<ExperimentTemplate, StartupScriptExecutor> _startupScriptComposer;
  private readonly ICommandComposer<ExperimentTemplate, CloseoutScriptExecutor> _closeoutScriptComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IEnumerable<IExecutionSummaryHandler> _summaryHandlers;
  private readonly AresVariableManager _variableManager;
  readonly AnalysisHelper _analysisHelper;
  readonly AnalysisRepo _analysisRepo;

  internal CampaignExecutor(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    ICommandComposer<ExperimentTemplate, StartupScriptExecutor> startupScriptComposer,
    ICommandComposer<ExperimentTemplate, CloseoutScriptExecutor> closeoutScriptComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    AnalysisHelper analysisHelper,
    CampaignTemplate template,
    IEnumerable<IExecutionSummaryHandler> resultHandlers,
    AresVariableManager variableManager,
    AnalysisRepo analysisRepo)
  {
    _analysisRepo = analysisRepo;
    _analysisHelper = analysisHelper;
    _variableManager = variableManager;
    _experimentComposer = experimentComposer;
    _startupScriptComposer = startupScriptComposer;
    _closeoutScriptComposer = closeoutScriptComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _summaryHandlers = resultHandlers;
    Template = template;

    Status = new CampaignExecutionStatus
    {
      CampaignId = template.UniqueId,
      State = ExecutionState.Waiting
    };

    _executionStatusSubject = new BehaviorSubject<CampaignExecutionStatus>(Status);
    ExperimentStatusObservable = _executionStatusSubject.AsObservable();
  }

  public async Task<CampaignExecutionSummary> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.Now;

    //Create Campaign Path
    var campaignPath = CreateCampaignExecutionSummariesFolder(startTime);
    AresEnvironment.AresEnvironment.SetEnvironmentVariable(VariableType.CampaignResultPath, campaignPath);

    //Create Miscellaneous Folder
    var miscFolderPath = CreateCampaignMiscellaneousFolder(campaignPath);
    AresEnvironment.AresEnvironment.SetEnvironmentVariable(VariableType.CampaignMiscFolder, miscFolderPath);

    //Create Startup Folder
    var startupFolder = CreateStartupSubFolder(campaignPath, "Startup");
    AresEnvironment.AresEnvironment.SetEnvironmentVariable(VariableType.CampaignStartupFolder, startupFolder);

    //Set Internal Variables related to Campaign
    AresEnvironment.AresEnvironment.SetInternalVariable(InternalVariableType.CurrentCampaignId, Template.UniqueId);
    AresEnvironment.AresEnvironment.SetInternalVariable(InternalVariableType.CurrentCampaignName, Template.Name);

    var experimentSummaries = new List<ExperimentExecutionSummary>();
    var analyses = new List<Analysis>();
    Status = new CampaignExecutionStatus
    {
      CampaignId = Template.UniqueId,
      State = ExecutionState.Waiting
    };

    _analysisRepo.ClearAnalyses();
    Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
    _executionReporter.Report(Status);

    var startupExecutor = GenerateStartupScriptExecutor(token.CancellationToken);
    await HandleExperimentStartup(token, startupExecutor);
    bool executionSuccess = true;
    var experiment_count = 0;

    while(!ShouldStop() && !token.IsCancelled)
    {
      var experimentFolder = $"Experiment_{++experiment_count}";
      var experimentPath = CreateExperimentSubFolder(campaignPath, experimentFolder);
      AresEnvironment.AresEnvironment.SetEnvironmentVariable(VariableType.ExperimentResultPath, experimentPath);

      //Populate Internal Variables Related to Experiment
      AresEnvironment.AresEnvironment.SetInternalVariable(InternalVariableType.CurrentExperimentNumber, experiment_count.ToString());

      var experimentExecutorResult = await GenerateExperimentExecutor(analyses, token.CancellationToken);
      if(experimentExecutorResult.ErrorString is not null || experimentExecutorResult.ExperimentExecutor is not ExperimentExecutor experimentExecutor)
        break;

      Status.ExperimentExecutionStatuses.Add(experimentExecutor.Status);
      experimentExecutor.ExperimentStatusObservable.Subscribe(experimentStatus =>
      {
        _executionReporter.Report(experimentStatus);
        Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
        _executionStatusSubject.OnNext(Status);
        _executionReporter.Report(Status);
      });

      var experimentSummary = await experimentExecutor.Execute(token);
      experimentSummary.ResultOutputPath = experimentPath;

      // if the execution was canceled, the experiment may not have executed the command to provide the output
      // and thus sending a null result to the analyzer might break it depending on the analyzer
      if(!token.IsCancelled)
      {
        var analysis = await _analysisHelper.Analyze(
          experimentExecutor.Template.Analyzer,
          experimentSummary,
          token.CancellationToken);
        analyses.Add(analysis);
        _analysisRepo.Add(analysis);
      }
      else
      {
        executionSuccess = false;
      }

      await PostExperimentExecution(experimentSummary);
      experimentSummaries.Add(experimentSummary);
    }

    var closeoutExecutor = GenerateCloseoutScriptExecutor(token.CancellationToken);
    await HandleExperimentCloseout(token, closeoutExecutor);

    if(executionSuccess)
      Status.State = ExecutionState.Succeeded;

    else
      Status.State = ExecutionState.Failed;

    _executionReporter.Report(Status);

    var campaignExecutionSummary = new CampaignExecutionSummary
    {
      UniqueId = Guid.NewGuid().ToString(),
      CampaignId = Template.UniqueId,
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = DateTime.UtcNow.ToTimestamp(),
        TimeStarted = startTime.ToUniversalTime().ToTimestamp()
      }
    };

    campaignExecutionSummary.ExperimentSummaries.AddRange(experimentSummaries);

    return campaignExecutionSummary;
  }

  private bool ShouldStop()
  {
    return StopConditions.Any(condition => condition.ShouldStop());
  }

  private string CreateCampaignExecutionSummariesFolder(DateTime startTime)
  {
    var newFolderName = $"{Template.Name}_{startTime.ToString("h-mm_M-dd")}";
    var fullPath = Path.Combine(AresConfig.ResultsPath, newFolderName);
    Directory.CreateDirectory(fullPath);
    return fullPath;
  }

  private string CreateCampaignMiscellaneousFolder(string campaignPath)
  {
    var newFolderPath = Path.Combine(campaignPath, "Miscellaneous");
    Directory.CreateDirectory(newFolderPath);
    return newFolderPath;
  }

  private string CreateExperimentSubFolder(string camapignPath, string folderName)
  {
    var experimentPath = Path.Combine(camapignPath, folderName);
    Directory.CreateDirectory(experimentPath);
    return experimentPath;
  }

  private string CreateStartupSubFolder(string campaignPath, string folderName)
  {
    var startupPath = Path.Combine(campaignPath, folderName);
    Directory.CreateDirectory(startupPath);
    return startupPath;
  }

  private async Task<ExperimentExecutorResult> GenerateExperimentExecutor(IEnumerable<Analysis> analyses, CancellationToken cancellationToken)
  {
    var result = new ExperimentExecutorResult();

    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = Template.ExperimentTemplates.First().CloneWithNewIds();
    if(!experimentTemplate.IsResolved())
    {
      if(ShouldReplan(analyses))
      {
        var resolveSuccess = await _planningHelper.TryResolveParameters(Template.PlannerAllocations, experimentTemplate.GetAllPlannedParameters(), analyses, cancellationToken);
        if(!resolveSuccess)
        {
          result.ErrorString = "Failed to plan! Experiment will be terminated!";
          return result;
        }
      }

      else
        experimentTemplate = analyses.Last().CompletedExperiment.Template.CloneWithNewIds();
    }

    if(!experimentTemplate.IsEnvironmentResolved())
    {
      var resolveVarsSuccess = _variableManager.TryResolveVariable(experimentTemplate.GetAllParameters());

      if(!resolveVarsSuccess)
      {
        result.ErrorString = "Failed to assign environment variables! Experiment will be terminated!";
        return result;
      }
    }

    //Passing the campaigns name into the experiment template for file creation purposes post experiment
    experimentTemplate.Name = Template.Name;

    result.ExperimentExecutor = _experimentComposer.Compose(experimentTemplate);
    return result;
  }

  private StartupScriptExecutor? GenerateStartupScriptExecutor(CancellationToken cancellationToken)
  {
    var experimentTemplate = Template.ExperimentTemplates.First().CloneWithNewIds();

    //Passing the campaigns name into the experiment template for file creation purposes post experiment
    experimentTemplate.Name = Template.Name;

    var resolveVarsSuccess = _variableManager.TryResolveVariable(experimentTemplate.GetAllStartupParameters());

    return _startupScriptComposer.Compose(experimentTemplate);
  }

  private CloseoutScriptExecutor? GenerateCloseoutScriptExecutor(CancellationToken cancellationToken)
  {
    var experimentTemplate = Template.ExperimentTemplates.First().CloneWithNewIds();

    //Passing the campaigns name into the experiment template for file creation purposes post experiment
    experimentTemplate.Name = Template.Name;

    return _closeoutScriptComposer.Compose(experimentTemplate);
  }

  private bool ShouldReplan(IEnumerable<Analysis> analyses)
  {
    var numberOfCompletedExperiments = analyses.Count();
    return numberOfCompletedExperiments % ReplanRate == 0;
  }

  private void RecallPreviousExperiment(IEnumerable<Analysis> analyses, ExperimentTemplate currentTemplate)
  {
    var previousExperiment = analyses.LastOrDefault();
  }

  public async Task HandleExperimentStartup(ExecutionControlToken token, StartupScriptExecutor? startupExecutor)
  {
    if(startupExecutor is null)
      throw new InvalidOperationException("Startup Executor returned null, cannot execute experiment!");

    startupExecutor.ExperimentStatusObservable.Subscribe(startupStatus =>
    {
      _executionReporter.Report(startupStatus);
      Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
      _executionStatusSubject.OnNext(Status);
      _executionReporter.Report(Status);
    });

    await startupExecutor.Execute(token);
  }

  public async Task HandleExperimentCloseout(ExecutionControlToken token, CloseoutScriptExecutor? closeoutExecutor)
  {
    if(closeoutExecutor is null)
      throw new InvalidOperationException("Closeout Executor returned null, cannot execute closeout script!");

    closeoutExecutor.ExperimentStatusObservable.Subscribe(closeoutStatus =>
    {
      _executionReporter.Report(closeoutStatus);
      Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
      _executionStatusSubject.OnNext(Status);
      _executionReporter.Report(Status);
    });

    await closeoutExecutor.Execute(token);
  }

  private async Task PostExperimentExecution(ExperimentExecutionSummary summary)
  {
    foreach(var handler in _summaryHandlers)
    {
      await handler.Handle(summary);
    }
  }

  public CampaignTemplate Template { get; }
  public IList<IStopCondition> StopConditions { get; } = new List<IStopCondition>();
  public double ReplanRate { get; set; } = 1;
  public IObservable<CampaignExecutionStatus> ExperimentStatusObservable { get; }
  public CampaignExecutionStatus Status { get; private set; }
}
