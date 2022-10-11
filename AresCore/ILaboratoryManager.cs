using Ares.Device;
using Ares.Messaging;

namespace Ares.Core;

public interface ILaboratoryManager
{
  // TODO: User, authentication/availability, etc.
  Laboratory Lab { get; }
  void RunCampaign(CampaignTemplate campaignTemplate);
  bool RegisterDeviceInterpreter(IDeviceCommandInterpreter<IAresDevice> deviceInterpreter);
  Project ActiveProject { get; }
}