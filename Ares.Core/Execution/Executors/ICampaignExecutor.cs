using Ares.Core.Execution.StopConditions;
using Ares.Messaging;
using Google.Protobuf;
using Grpc.Core;

namespace Ares.Core.Execution.Executors;

public interface ICampaignExecutor : IExecutor<CampaignResult, CampaignExecutionStatus>
{
  IList<IStopCondition> StopConditions { get; }
  double ReplanRate { get; set; }
}
