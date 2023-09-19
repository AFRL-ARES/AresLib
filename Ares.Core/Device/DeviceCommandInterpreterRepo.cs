using System.Collections.ObjectModel;
using Ares.Device;
using CoreDevice;

namespace Ares.Core.Device;

internal class DeviceCommandInterpreterRepo : Collection<IDeviceCommandInterpreter<IAresDevice>>, IDeviceCommandInterpreterRepo
{
  public DeviceCommandInterpreterRepo()
  {
    var coreDevice = new AresCoreDevice();
    var coreInterpreter = new AresCoreDeviceCommandInterpreter(coreDevice);
    Add(coreInterpreter);
  }
}
