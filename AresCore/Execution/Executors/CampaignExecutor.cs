using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Composers;
using Ares.Core.Execution.Extensions;
using Ares.Core.Planning;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class CampaignExecutor : IExecutor<CampaignResult, CampaignExecutionStatus>
{
  private readonly CancellationTokenSource _cancellationTokenSource;
  private readonly IExecutionReporter _executionReporter;
  private readonly ISubject<CampaignExecutionStatus> _executionStatusSubject;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;

  public CampaignExecutor(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    CampaignTemplate template)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    Template = template;
    _cancellationTokenSource = new CancellationTokenSource();

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

  public async Task<CampaignResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();
    Status.State = ExecutionState.Running;
    _executionReporter.Report(Status);
    while (!ShouldStop() && !cancellationToken.IsCancellationRequested)
    {
      var experimentExecutor = await GenerateExperimentExecutor();
      if (experimentExecutor is null)
        break;

      Status.ExperimentExecutionStatuses.Add(experimentExecutor.Status);
      experimentExecutor.StatusObservable.Subscribe(experimentStatus => {
        _executionStatusSubject.OnNext(Status);
        _executionReporter.Report(experimentStatus);
        _executionReporter.Report(Status);
      });

      var experimentResult = await experimentExecutor.Execute(_cancellationTokenSource.Token);
      experimentResults.Add(experimentResult);
    }

    Status.State = ExecutionState.Succeeded;
    _executionReporter.Report(Status);
    var campaignResult = new CampaignResult
    {
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

  private async Task<ExperimentExecutor?> GenerateExperimentExecutor()
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = Template.ExperimentTemplates.First();
    if (!experimentTemplate.IsResolved())
    {
      var resolveSuccess = await _planningHelper.TryResolveParameters(Template.PlannerAllocations, experimentTemplate.GetAllPlannedParameters());
      if (!resolveSuccess)
        return null;
    }

    return _experimentComposer.Compose(experimentTemplate);
  }
}
