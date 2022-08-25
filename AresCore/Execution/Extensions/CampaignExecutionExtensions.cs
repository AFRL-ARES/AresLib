using Ares.Messaging;

namespace Ares.Core.Execution.Extensions;

internal static class CampaignExecutionExtensions
{
  public static IEnumerable<CommandExecutionStatus> GetAllCommandStatuses(this CampaignExecutionStatus campaignExecutionStatus)
  {
    return campaignExecutionStatus.ExperimentExecutionStatuses
      .SelectMany(status => status.StepExecutionStatuses)
      .SelectMany(status => status.CommandExecutionStatuses);
  }
}
