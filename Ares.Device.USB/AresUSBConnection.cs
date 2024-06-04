using Ares.Device.USB.Commands;

namespace Ares.Device.USB;
public abstract class AresUSBConnection
{
  protected abstract void SendOutboundMessage(USBCommand command);
}
