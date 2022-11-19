using System;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device.Serial;

public abstract class SerialDevice<TConnection> : AresDevice, ISerialDevice<TConnection> where TConnection : IAresSerialConnection
{
  protected SerialDevice(string name, TConnection connection) : base(name)
  {
    Connection = connection;
  }

  public TConnection Connection { get; }

  public override Task<bool> Activate()
    => SerialActivate();

  private async Task<bool> SerialActivate()
  {
    if (!Connection.IsOpen)
    {
      Connection.AttemptOpen();
      var errorMessage = $"Established connection {Connection.Name} failed to report being open";
      Status = new DeviceStatus
      {
        DeviceState = DeviceState.Error,
        Message = errorMessage
      };
    }

    try
    {
      var validationResult = await Validate();
      if (!validationResult.Success)
      {
        Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = $"{Name} connected but could not pass validation.{Environment.NewLine}{validationResult.Message}" };
        return false;
      }
    }
    catch (Exception e)
    {
      Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = e.Message };
      return false;
    }

    Status = new DeviceStatus { DeviceState = DeviceState.Active, Message = $"Activated {Name}" };
    return true;
  }

  protected abstract Task<DeviceValidationResult> Validate();
}
