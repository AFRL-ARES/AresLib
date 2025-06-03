using Ares.Device;
using CoreDevice;
using System.Collections.Concurrent;

namespace Ares.Core.Device;

public class DeviceCommandInterpreterRepo : SynchronizedCollection<IDeviceCommandInterpreter<IAresDevice>>, IDeviceCommandInterpreterRepo
{
  private ConcurrentBag<IDeviceCommandInterpreter<IAresDevice>> _bag = new();
  private IEnumerable<IDeviceConfirmationRequestHandler> _deviceConfirmationHandler;

  public DeviceCommandInterpreterRepo(IEnumerable<IDeviceConfirmationRequestHandler> confirmationServices)
  {
    _deviceConfirmationHandler = confirmationServices;
    var coreDevice = new AresCoreDevice(_deviceConfirmationHandler);
    var coreInterpreter = new AresCoreDeviceCommandInterpreter(coreDevice);
    Add(coreInterpreter);
  }
}
