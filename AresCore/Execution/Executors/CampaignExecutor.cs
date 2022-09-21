using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Analyzing;
using Ares.Core.Composers;
using Ares.Core.Execution.Extensions;
using Ares.Core.Planning;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class CampaignExecutor : IExecutor<CampaignResult, CampaignExecutionStatus>
{
  private readonly IAnalyzer _analyzer;
  // private readonly CancellationTokenSource _cancellationTokenSource;
  private readonly IExecutionReporter _executionReporter;
  private readonly ISubject<CampaignExecutionStatus> _executionStatusSubject;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;

  public CampaignExecutor(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    IAnalyzer analyzer,
    CampaignTemplate template)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _analyzer = analyzer;
    Template = template;
    // _cancellationTokenSource = new CancellationTokenSource();

    Status = new CampaignExecutionStatus
    {
      CampaignId = template.UniqueId,
      State = ExecutionState.Waiting
    };

    _executionStatusSubject = new BehaviorSubject<CampaignExecutionStatus>(Status);
    StatusObservable = _executionStatusSubject.AsObservable();
  }

  public CampaignTemplate Template { get; set; }

  public IObservable<CampaignExecutionStatus> StatusObservable { get; }
  public CampaignExecutionStatus Status { get; }

  public async Task<CampaignResult> Execute(CancellationToken cancellationToken, PauseToken pauseToken)
  {
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();
    var analyses = new List<Analysis>();
    Status.State = pauseToken.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
    _executionReporter.Report(Status);

    while (!ShouldStop() && !cancellationToken.IsCancellationRequested)
    {
      var experimentExecutor = await GenerateExperimentExecutor(analyses, cancellationToken);
      if (experimentExecutor is null)
        break;

      Status.ExperimentExecutionStatuses.Add(experimentExecutor.Status);
      experimentExecutor.StatusObservable.Subscribe(experimentStatus => {
        _executionStatusSubject.OnNext(Status);
        _executionReporter.Report(experimentStatus);
        _executionReporter.Report(Status);
      });

      var experimentResult = await experimentExecutor.Execute(cancellationToken, pauseToken);
      experimentResults.Add(experimentResult);

      // if the execution was canceled, the experiment may not have executed the command to provide the output
      // and thus sending a null result to the analyzer might break it depending on the analyzer
      if (!cancellationToken.IsCancellationRequested)
      {
        var analysis = await _analyzer.Analyze(experimentResult.CompletedExperiment, cancellationToken);
        analyses.Add(analysis);
      }
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
    => false;

  private async Task<ExperimentExecutor?> GenerateExperimentExecutor(IEnumerable<Analysis> analyses, CancellationToken cancellationToken)
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = Template.ExperimentTemplates.First().CloneWithNewIds();
    if (!experimentTemplate.IsResolved())
    {
      var resolveSuccess = await _planningHelper.TryResolveParameters(Template.PlannerAllocations, experimentTemplate.GetAllPlannedParameters(), analyses, cancellationToken);
      if (!resolveSuccess)
        return null;
    }

    return _experimentComposer.Compose(experimentTemplate);
  }
}
