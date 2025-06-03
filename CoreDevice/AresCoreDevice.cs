using Ares.Device;
using Ares.Messaging.Device;

namespace CoreDevice;

public class AresCoreDevice : IAresDevice
{
  private IEnumerable<IDeviceConfirmationRequestHandler> _confirmationHandler;
  public AresCoreDevice(IEnumerable<IDeviceConfirmationRequestHandler> confirmationHandler)
  {
    _confirmationHandler = confirmationHandler;
  }

  public string Name => "ARES";

  public DeviceStatus Status { get; } = new DeviceStatus { DeviceState = DeviceState.Active };

  public Task<bool> Activate()
  {
    return Task.FromResult(true);
  }

  public Task Sleep(TimeSpan timeSpan)
  {
    return Task.Delay(timeSpan);
  }

  public async Task WaitForUser(string message)
  {
    var confirmationHelper = _confirmationHandler.FirstOrDefault();

    if(confirmationHelper is not null)
      await confirmationHelper.RequestConfirmation(message);
  }
}
