using Ares.Device.Serial;

namespace Ares.Device.USB;
public interface IUsbDevice<out TConnection> : IAresDevice where TConnection : IAresUSBConnection
{
  TConnection Connection { get; }
}
