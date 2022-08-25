using Ares.Messaging;

namespace Ares.Core.Execution;

public interface IExecutionReporter
{
  IObservable<CampaignExecutionStatus?> CampaignStatusObservable { get; }
  IObservable<ExperimentExecutionStatus?> ExperimentStatusObservable { get; }
  CampaignExecutionStatus? CampaignExecutionStatus { get; }
  ExperimentExecutionStatus? ExperimentExecutionStatus { get; }
  internal void Report(CampaignExecutionStatus status);
  internal void Report(ExperimentExecutionStatus status);
}
