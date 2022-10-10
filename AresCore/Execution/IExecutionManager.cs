using Ares.Core.Execution.StopConditions;
using Ares.Messaging;

namespace Ares.Core.Execution;

public interface IExecutionManager
{
  public IList<IStopCondition>? CampaignStopConditions { get; }
  public bool CanRun { get; }
  void LoadTemplate(CampaignTemplate template);
  void Start();
  void Stop();
  void Pause();
  void Resume();
}
