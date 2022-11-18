namespace Ares.Device.Serial;

public interface ISerialDevice<TConnection> : IAresDevice where TConnection : IAresSerialPort
{
}
