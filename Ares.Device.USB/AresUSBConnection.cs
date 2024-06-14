using Ares.Device.Serial;
using Ares.Device.USB.Commands;

namespace Ares.Device.USB;
public abstract class AresUSBConnection : IAresUSBConnection
{
  protected internal AresUSBConnection(USBConnectionInfo connectionInfo, string name)
  {
    ConnectionInfo = connectionInfo;
    Name = name;
  }

  public string? Name { get; set; }

  public bool IsOpen { get; protected set; }

  protected USBConnectionInfo ConnectionInfo { get; }

  protected abstract void Open(string portName);

  public void AttemptOpen()
  {
    throw new NotImplementedException();
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }

  public Task Send(USBCommand command)
  {
    throw new NotImplementedException();
  }

  protected abstract void SendOutboundMessage(USBCommand command);
}
