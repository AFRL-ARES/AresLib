using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Messaging;

namespace Ares.Core.Execution;

public class ExecutionReporter : IExecutionReporter
{
  private readonly ISubject<CampaignExecutionStatus?> _campaignExecutionStatusSubject = new BehaviorSubject<CampaignExecutionStatus?>(null);
  private readonly ISubject<ExperimentExecutionStatus?> _experimentExecutionStatusSubject = new BehaviorSubject<ExperimentExecutionStatus?>(null);

  public ExecutionReporter()
  {
    CampaignStatusObservable = _campaignExecutionStatusSubject.AsObservable();
    ExperimentStatusObservable = _experimentExecutionStatusSubject.AsObservable();
  }

  public IObservable<CampaignExecutionStatus?> CampaignStatusObservable { get; }
  public IObservable<ExperimentExecutionStatus?> ExperimentStatusObservable { get; }
  public CampaignExecutionStatus? CampaignExecutionStatus { get; private set; }
  public ExperimentExecutionStatus? ExperimentExecutionStatus { get; private set; }

  void IExecutionReporter.Report(CampaignExecutionStatus status)
  {
    _campaignExecutionStatusSubject.OnNext(status);
    CampaignExecutionStatus = status;
  }

  void IExecutionReporter.Report(ExperimentExecutionStatus status)
  {
    _experimentExecutionStatusSubject.OnNext(status);
    ExperimentExecutionStatus = status;
  }
}
