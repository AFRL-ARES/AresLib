using System;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial.Simulation;

public abstract class AresSimPort : AresSerialPort
{
  protected AresSimPort(SerialPortConnectionInfo connectionInfo) : base(connectionInfo)
  {
  }

  protected override void Open(string portName)
  {
    if (!portName.StartsWith("SIM"))
      throw new InvalidOperationException(
        $"Tried opening simulated port of type {GetType().Name} with port name {portName}. Simulated ports may only open on ports starting with SIM");

    IsOpen = true;
  }

  public abstract void SendInternally(byte[] bytes);

  public override void SendOutboundMessage(SerialCommand command)
  {
    SendInternally(command.SerializedData);
  }

  protected internal override void CloseCore()
  {
    IsOpen = false;
  }
}
