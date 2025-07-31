using Ares.Device;
using Ares.Messaging.Device;

namespace Ares.Core.CoreDevice;

public class AresCoreDevice : IAresDevice
{
  public AresCoreDevice()
  {
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

  public Task WaitForUser(string message)
  {
    //var confirmationHelper = _confirmationHandler.FirstOrDefault();

    //if(confirmationHelper is not null)
    //await confirmationHelper.RequestConfirmation(message);
    return Task.CompletedTask;
  }
}
