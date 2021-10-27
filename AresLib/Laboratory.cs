using AresLib.Device;
using System.Collections.ObjectModel;

namespace AresLib
{
  public class Laboratory
  {
    public Laboratory(string name, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> deviceInterpreters)
    {
      Name = name;
      DeviceInterpreters = deviceInterpreters;
    }

    public string Name { get; }

    public ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> DeviceInterpreters { get; }
  }
}
