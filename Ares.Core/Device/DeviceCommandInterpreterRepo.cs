using Ares.Device;
using CoreDevice;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Ares.Core.Device;

public class DeviceCommandInterpreterRepo : SynchronizedCollection<IDeviceCommandInterpreter<IAresDevice>>, IDeviceCommandInterpreterRepo
{
  private ConcurrentBag<IDeviceCommandInterpreter<IAresDevice>> _bag = new();
  public DeviceCommandInterpreterRepo()
  {
    var coreDevice = new AresCoreDevice();
    var coreInterpreter = new AresCoreDeviceCommandInterpreter(coreDevice);
    Add(coreInterpreter);
  }
}
