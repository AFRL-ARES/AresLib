using System.Threading.Tasks;
using Ares.Core.Messages;
using AresDevicePluginBase;
using AresLib.Builders;

namespace AresLib
{
  public interface ILaboratoryManager
  {
    // TODO: User, authentication/availability, etc.
    Laboratory Lab { get; }
    ICampaignTemplateBuilder GenerateCampaignBuilder(string campaignName);
    void RunCampaign(CampaignTemplate campaignTemplate);
    Task<bool> RegisterDeviceInterpreter(IDeviceCommandInterpreter<AresDevice> deviceInterpreter);
  }
}
