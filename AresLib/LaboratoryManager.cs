using AresLib.Builders;
using AresLib.Composers;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ares.Core.Messages;
using AresDevicePluginBase;

namespace AresLib
{
  public abstract class LaboratoryManager : ILaboratoryManager
  {
    protected LaboratoryManager(string name)
    {
      Name = name;
      DeviceCommandInterpretersSource
        .Connect()
        .Bind(out var availableDeviceCommandInterpreters)
        .Subscribe();

      AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
      Lab = new Laboratory(Name, AvailableDeviceCommandInterpreters);
    }

    protected abstract IDeviceCommandInterpreter<IAresDevice>[] GenerateDeviceCommandInterpreters();

    public Laboratory Lab { get; }

    public ICampaignTemplateBuilder GenerateCampaignBuilder(string name)
    {
      var campaignBuilder = new CampaignTemplateBuilder(name);
      return campaignBuilder;
    }

    public void RunCampaign(CampaignTemplate campaignTemplate)
    {

      var campaignComposer = new CampaignComposer(campaignTemplate, AvailableDeviceCommandInterpreters);
      var campaignExecutor = campaignComposer.Compose();
      Task.Run(() => campaignExecutor.Execute()).Wait();
    }

    public async Task<bool> RegisterDeviceInterpreter(IDeviceCommandInterpreter<IAresDevice> deviceInterpreter)
    {
      var deviceActivated = await deviceInterpreter.Device.Activate();
      if (!deviceActivated)
      {
        throw new Exception($"Could not activate device, not going to register");
      }
      DeviceCommandInterpretersSource.AddOrUpdate(deviceInterpreter);
      return true;
    }

    protected ISourceCache<IDeviceCommandInterpreter<IAresDevice>, string> DeviceCommandInterpretersSource { get; }
      = new SourceCache<IDeviceCommandInterpreter<IAresDevice>, string>(interpreter => interpreter.Device.Name);

    private ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> AvailableDeviceCommandInterpreters { get; }

    public string Name { get; }
  }
}
