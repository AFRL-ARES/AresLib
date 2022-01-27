namespace Ares.Device.Serial;

public interface ISerialDevice : IAresDevice
{
  ISerialConnection Connection { get; }
  void Connect(string portName);
  void Disconnect();
}
