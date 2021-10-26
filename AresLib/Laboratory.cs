using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.Device;
using DynamicData;
using DynamicData.Binding;

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
