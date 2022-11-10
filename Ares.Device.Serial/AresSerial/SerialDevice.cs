using System;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device.Serial;

public abstract class SerialDevice<TConnection> : AresDevice, ISerialDevice<TConnection> where TConnection : IAresSerialPort
{
  protected SerialDevice(string name) : base(name)
  {
  }

  public TConnection? Connection { get; private set; }

  public Task Connect(TConnection serialPort, string portName)
  {
    // TODO: Are there edge cases... Maybe we already have a connection that we need to check?
    if (Connection != null)
      Disconnect();

    Connection = serialPort;
    try
    {
      Connection.AttemptOpen(portName);
    }
    catch (Exception e)
    {
      Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = e.Message };
      throw;
    }

    return OnConnected();
  }

  public void Disconnect()
  {
    if (Connection == null)
      return;

    Connection.Close();
    Status = new DeviceStatus { DeviceState = DeviceState.Inactive, Message = "Explicitly disconnected" };
    OnDisconnected();
  }

  public override async Task<bool> Activate()
  {
    if (Connection is null)
      throw new Exception("Cannot activate serial device before providing connection.");

    if (!Connection.IsOpen)
    {
      var errorMessage = $"Established connection {Connection.Name} failed to report being open";
      Status = new DeviceStatus
      {
        DeviceState = DeviceState.Error,
        Message = errorMessage
      };

      throw new Exception(errorMessage);
    }

    try
    {
      if (!await Validate())
      {
        Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = $"{Name} connected but could not pass validation. Wrong target device?" };
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


  protected virtual Task OnConnected()
    => Task.CompletedTask;

  protected virtual Task OnDisconnected()
    => Task.CompletedTask;

  protected abstract Task<bool> Validate();
}
