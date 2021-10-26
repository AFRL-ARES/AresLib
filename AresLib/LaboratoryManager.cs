using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Builders;
using AresLib.Device;
using DynamicData;

namespace AresLib
{
  public abstract class LaboratoryManager : ILaboratoryManager
  {
    protected LaboratoryManager()
    {
      Lab = BuildLab();
    }

    protected abstract Laboratory BuildLab();


    protected ISourceCache<IDeviceCommandInterpreter<AresDevice>, string> DeviceCommandInterpretersSource { get; } 
      = new SourceCache<IDeviceCommandInterpreter<AresDevice>, string>(interpreter => interpreter.Device.Name);
    public Laboratory Lab { get; }

    public ITemplateBuilder<CampaignTemplate> GenerateCampaignBuilder(string name)
    {
      var campaignBuilder = new CampaignTemplateBuilder(name);
      return campaignBuilder;
    }
  }
}
