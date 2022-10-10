using Ares.Messaging;

namespace Ares.Core.Execution.StartConditions;

public class CampaignInProgressStartCondition : IStartCondition
{
  private readonly IExecutionReporter _executionReporter;

  public CampaignInProgressStartCondition(IExecutionReporter executionReporter)
  {
    _executionReporter = executionReporter;
  }

  public string Message => $"Campaign with id {_executionReporter.CampaignExecutionStatus?.CampaignId} is currently running.";

  public bool CanStart()
  {
    var state = _executionReporter.CampaignExecutionStatus?.State;
    if (state is null)
      return true;

    return state != ExecutionState.Running && state != ExecutionState.Paused;
  }
}
