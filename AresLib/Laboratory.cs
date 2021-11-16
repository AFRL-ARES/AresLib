using System.Collections.ObjectModel;
using AresDevicePluginBase;

namespace AresLib
{
  public class Laboratory
  {
    public Laboratory(string name, ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> deviceInterpreters)
    {
      Name = name;
      DeviceInterpreters = deviceInterpreters;
    }

    public string Name { get; }

    public ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> DeviceInterpreters { get; }
  }
}
