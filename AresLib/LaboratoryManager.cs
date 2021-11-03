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
      var deviceCommandInterpreters = GenerateDeviceCommandInterpreters();
      DeviceCommandInterpretersSource.AddOrUpdate(deviceCommandInterpreters);
    }

    protected abstract IDeviceCommandInterpreter<AresDevice>[] GenerateDeviceCommandInterpreters();

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

    protected ISourceCache<IDeviceCommandInterpreter<AresDevice>, string> DeviceCommandInterpretersSource { get; }
      = new SourceCache<IDeviceCommandInterpreter<AresDevice>, string>(interpreter => interpreter.Device.Name);

    private ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> AvailableDeviceCommandInterpreters { get; }

    public string Name { get; }
  }
}
