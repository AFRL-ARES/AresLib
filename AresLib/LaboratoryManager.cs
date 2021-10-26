using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Builders;
using AresLib.Composers;
using AresLib.Device;
using AresLib.Executors;
using DynamicData;

namespace AresLib
{
  public abstract class LaboratoryManager : ILaboratoryManager
  {
    protected LaboratoryManager()
    {
      DeviceCommandInterpretersSource
        .Connect()
        .Bind(out var availableDeviceCommandInterpreters)
        .Subscribe();

      AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
      Lab = BuildLab();
    }

    protected abstract Laboratory BuildLab();



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

    private ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> AvailableDeviceCommandInterpreters {get;}
  }
}
