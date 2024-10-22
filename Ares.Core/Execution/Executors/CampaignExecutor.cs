using Ares.Core.Analyzing;
using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Executors.Composers;
using Ares.Core.Execution.Extensions;
using Ares.Core.Execution.StopConditions;
using Ares.Core.Planning;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ares.Core.Execution.Executors;

public class CampaignExecutor : ICampaignExecutor
{
  private readonly IAnalyzerManager _analyzerManager;
  private readonly IExecutionReporter _executionReporter;
  private readonly ISubject<CampaignExecutionStatus> _executionStatusSubject;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IEnumerable<IResultHandler> _resultHandlers;

  public CampaignExecutor(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    IAnalyzerManager analyzerManager,
    CampaignTemplate template,
    IEnumerable<IResultHandler> resultHandlers)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _analyzerManager = analyzerManager;
    _resultHandlers = resultHandlers;
    Template = template;

    Status = new CampaignExecutionStatus
    {
      CampaignId = template.UniqueId,
      State = ExecutionState.Waiting
    };

    _executionStatusSubject = new BehaviorSubject<CampaignExecutionStatus>(Status);
    StatusObservable = _executionStatusSubject.AsObservable();
  }

  public async Task<CampaignResult> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();
    var analyses = new List<Analysis>();
    Status = new CampaignExecutionStatus
    {
      CampaignId = Template.UniqueId,
      State = ExecutionState.Waiting
    };

    _analyzerManager.ClearAnalyses();
    Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
    _executionReporter.Report(Status);

    while (!ShouldStop() && !token.IsCancelled)
    {
      var experimentExecutor = await GenerateExperimentExecutor(analyses, token.CancellationToken);
      if (experimentExecutor is null)
        break;

      Status.ExperimentExecutionStatuses.Add(experimentExecutor.Status);
      experimentExecutor.StatusObservable.Subscribe(experimentStatus =>
      {
        _executionStatusSubject.OnNext(Status);
        _executionReporter.Report(experimentStatus);
        _executionReporter.Report(Status);
      });

      var experimentResult = await experimentExecutor.Execute(token);

      // if the execution was canceled, the experiment may not have executed the command to provide the output
      // and thus sending a null result to the analyzer might break it depending on the analyzer
      if (!token.IsCancelled)
      {
        var noneAnalyzer = _analyzerManager.GetAnalyzer<NoneAnalyzer>();
        var analyzer = experimentExecutor.Template.Analyzer is null ? noneAnalyzer : _analyzerManager
          .GetAnalyzer(experimentExecutor.Template.Analyzer) ?? throw new InvalidOperationException($"Could not find desired Analyzer! {experimentExecutor.Template.Analyzer.Name}");

        var analysis = await analyzer.Analyze(experimentResult.CompletedExperiment.Result, token.CancellationToken);
        analysis.CompletedExperiment = experimentResult.CompletedExperiment;
        analyses.Add(analysis);
        _analyzerManager.StoreAnalysis(analysis);
      }

      await PostExperimentExecution(experimentResult);
      experimentResults.Add(experimentResult);
    }

    Status.State = ExecutionState.Succeeded;
    _executionReporter.Report(Status);

    var campaignResult = new CampaignResult
    {
      UniqueId = Guid.NewGuid().ToString(),
      CampaignId = Template.UniqueId,
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = DateTime.UtcNow.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      }
    };

    campaignResult.ExperimentResults.AddRange(experimentResults);

    return campaignResult;
  }

  private bool ShouldStop()
  {
    return StopConditions.Any(condition => condition.ShouldStop());
  }

  private async Task<ExperimentExecutor?> GenerateExperimentExecutor(IEnumerable<Analysis> analyses, CancellationToken cancellationToken)
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = Template.ExperimentTemplates.First().CloneWithNewIds();
    if (!experimentTemplate.IsResolved())
    {
      if (ShouldReplan(analyses))
      {
        var resolveSuccess = await _planningHelper.TryResolveParameters(Template.PlannerAllocations, experimentTemplate.GetAllPlannedParameters(), analyses, cancellationToken);
        if (!resolveSuccess)
          return null;
      }

      else
        experimentTemplate = analyses.Last().CompletedExperiment.Template.CloneWithNewIds();

    }

    //Passing the campaigns name into the experiment template for file creation purposes post experiment
    experimentTemplate.Name = Template.Name;

    return _experimentComposer.Compose(experimentTemplate);
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

  private async Task PostExperimentExecution(ExperimentResult result)
  {
    foreach (var handler in _resultHandlers)
    {
      await handler.Handle(result);
    }
  }

  public CampaignTemplate Template { get; }
  public IList<IStopCondition> StopConditions { get; } = new List<IStopCondition>();
  public double ReplanRate { get; set; } = 1;
  public IObservable<CampaignExecutionStatus> StatusObservable { get; }
  public CampaignExecutionStatus Status { get; private set; }
}
