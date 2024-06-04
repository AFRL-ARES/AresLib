using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial.Simulation;

public abstract class AresSerialSimConnection : AresSerialConnection
{
  protected AresSerialSimConnection(SerialPortConnectionInfo connectionInfo, string portName, SerialConnectionOptions? connectionOptions = null) : base(connectionInfo, portName, connectionOptions)
  {
  }

  protected override void Open(string portName)
  {
    IsOpen = true;
  }

  public abstract void SendInternally(byte[] bytes);

  protected override void SendOutboundMessage(SerialCommand command)
  {
    SendInternally(command.SerializedData);
  }

  protected internal override void CloseCore()
  {
    IsOpen = false;
  }
}
