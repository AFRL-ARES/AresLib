using Ares.Messaging;

namespace Ares.Core.Execution.StartConditions;

/// <summary>
/// A simple condition that checks whether or not the experiment is currently running
/// </summary>
internal class CampaignInProgressStartCondition : IStartCondition
{
  private readonly IExecutionReportStore _executionReportStore;

  public CampaignInProgressStartCondition(IExecutionReportStore executionReportStore)
  {
    _executionReportStore = executionReportStore;
  }

  public string Message => $"Campaign with id {_executionReportStore.CampaignExecutionStatus?.CampaignId} is currently in progress.";

  public bool CanStart()
  {
    var state = _executionReportStore.CampaignExecutionStatus?.State;
    if (state is null)
      return true;

    return state != ExecutionState.Running && state != ExecutionState.Paused;
  }
}
