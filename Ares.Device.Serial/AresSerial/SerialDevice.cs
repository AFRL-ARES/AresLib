using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device.Serial;

public abstract class SerialDevice<TConnection> : AresDevice, ISerialDevice<TConnection> where TConnection : IAresSerialPort
{
  protected SerialDevice(string name) : base(name)
  {
  }

  public void Connect(TConnection serialPort, string portName)
  {
    // TODO: Are there edge cases... Maybe we already have a connection that we need to check?
    if (Connection != null)
    {
      Disconnect();
    }
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
    OnConnected();
  }

  public void Disconnect()
  {
    if (Connection == null)
    {
      return;
    }
    Connection.Close();
    Status = new DeviceStatus { DeviceState = DeviceState.Inactive, Message = "Explicitly disconnected" };
    OnDisconnected();
  }

  public override bool Activate()
  {
    if (Connection is null)
      throw new Exception("Cannot activate serial device before providing connection.");
    if (!Connection.IsOpen)
    {
      // Connect(Connection, Connection.Name);
      // if (!Connection.IsOpen)
      // {
        var errorMessage = $"Established connection {Connection.Name} failed to report being open";
        Status = new DeviceStatus
        {
          DeviceState = DeviceState.Error,
          Message = errorMessage
        };

        throw new Exception(errorMessage);
      // }
    }
    try
    {
      if (!Validate())
      {
        Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = $"{Name} connected but could not pass validation. Wrong target device?"};
        return false;
      }
    }
    catch (Exception e)
    {
      Status = new DeviceStatus { DeviceState = DeviceState.Error, Message = e.Message};
      return false;
    }

    Status = new DeviceStatus { DeviceState = DeviceState.Active, Message = $"Activated {Name}"};
    return true;
  }


  protected virtual void OnConnected()
  {

  }

  protected virtual void OnDisconnected()
  {

  }

  protected abstract bool Validate();

  public TConnection? Connection { get; private set; }
}
