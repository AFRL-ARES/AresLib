using Ares.Device.Serial;
using Ares.Messaging.Device;

namespace Ares.Device.USB;
public abstract class USBDevice<TConnection> : AresDevice, IUsbDevice<TConnection> where TConnection : IAresUSBConnection
{
  protected USBDevice(string name, TConnection connection) : base(name)
  {
    Connection = connection;
  }

  public TConnection Connection { get; }

  public override Task<bool> Activate() => USBActivate();

  private async Task<bool> USBActivate()
  {
    if (!Connection.IsOpen)
    {
      try
      {
        Connection.AttemptOpen();
      }

      catch (Exception ex)
      {
        Status = new DeviceStatus
        {
          DeviceState = DeviceState.Error,
          Message = $"Failed to open connection {Connection.Name}{Environment.NewLine}{ex.Message}"
        };

        return false;
      }

      Status = new DeviceStatus
      {
        DeviceState = DeviceState.Error,
        Message = $"Successfully established connection {Connection.Name} but it failed to report as being open."
      };
    }

    try
    {
      var validationResult = await Validate();
      if (!validationResult.Success)
      {
        Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = $"{Name} connected but could not pass validation. {Environment.NewLine}{validationResult.Message}" };
        return false;
      }
    }

    catch (Exception ex)
    {
      Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = ex.Message };
      return false;
    }

    Status = new DeviceStatus { DeviceState = DeviceState.Active, Message = $"Activated {Name}" };
    return true;
  }

  protected abstract Task<USBDeviceValidationResult> Validate();
}
