using Ares.Core.Execution.StopConditions;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public interface ICampaignExecutor : IExecutor<CampaignResult, CampaignExecutionStatus>
{
  IList<IStopCondition> StopConditions { get; }
  double ReplanRate { get; set; }
  void UpdateExecutionNotes(string executionNotes);

  void UpdateCampaignTags(List<string> campaignTags);
}
