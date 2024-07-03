using Ares.Device.USB.Commands;

namespace Ares.Device.USB.Simulation;
public abstract class AresUSBSimConnection : AresUSBConnection
{
  protected AresUSBSimConnection(string name, USBConnectionInfo connectionInfo) : base(name, connectionInfo)
  {
  }

  public abstract void SendInternally(USBCommand command);

  protected override void SendOutboundMessage(USBCommand command)
  {
    SendInternally(command);
  }

  protected override void Open(string deviceName)
  {
    IsOpen = true;
  }

}
