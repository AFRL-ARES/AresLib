using Ares.Messaging;

namespace Ares.Core.Execution;

public interface IExecutionManager
{
  void LoadTemplate(CampaignTemplate template);
  void Start();
  void Stop();
  void Pause();
}
