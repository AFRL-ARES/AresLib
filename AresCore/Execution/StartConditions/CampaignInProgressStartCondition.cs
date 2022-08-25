using System.Reactive.Linq;
using Ares.Messaging;

namespace Ares.Core.Execution.StartConditions;

public class CampaignInProgressStartCondition : IStartCondition
{
  public CampaignInProgressStartCondition(IExecutionReporter executionReporter)
  {
    CanStartObservable = executionReporter.CampaignStatusObservable
      .Select(CanCampaignRun);
  }

  public IObservable<bool> CanStartObservable { get; }

  private static bool CanCampaignRun(CampaignExecutionStatus? status)
  {
    if (status is null)
      return true;

    return status.State != ExecutionState.Running && status.State != ExecutionState.Paused;
  }
}
