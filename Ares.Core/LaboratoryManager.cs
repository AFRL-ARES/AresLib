using System.Collections.ObjectModel;
using Ares.Device;
using Ares.Messaging;
using Ares.Messaging.Device;
using DynamicData;

namespace Ares.Core;

public abstract class LaboratoryManager : ILaboratoryManager
{
  protected LaboratoryManager(string name)
  {
    DeviceCommandInterpretersSource
      .Connect()
      .Bind(out var availableDeviceCommandInterpreters)
      .Subscribe();

    Name = name;
    AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
    Lab = new Laboratory(Name, AvailableDeviceCommandInterpreters);
    // TODO: TryLoad user's most recent active project
  }

  internal ISourceCache<IDeviceCommandInterpreter<IAresDevice>, string> DeviceCommandInterpretersSource { get; }
    = new SourceCache<IDeviceCommandInterpreter<IAresDevice>, string>(interpreter => interpreter.Device.Name);

  private ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> AvailableDeviceCommandInterpreters { get; }

  public string Name { get; }

  public Laboratory Lab { get; }

  public void RunCampaign(CampaignTemplate campaignTemplate)
  {
    // var campaignComposer = new CampaignComposer(campaignTemplate, AvailableDeviceCommandInterpreters);
    // var campaignExecutor = campaignComposer.Compose();
    // Task.Run(() => campaignExecutor.Execute())
    //   .Wait();
  }

  public void RegisterDeviceInterpreter(IDeviceCommandInterpreter<IAresDevice> deviceInterpreter)
  {
    DeviceCommandInterpretersSource.AddOrUpdate(deviceInterpreter);
  }

  public Project ActiveProject { get; set; }

  protected abstract IDeviceCommandInterpreter<IAresDevice>[] GenerateDeviceCommandInterpreters();
}
