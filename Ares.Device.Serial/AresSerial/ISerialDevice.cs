namespace Ares.Device.Serial;

public interface ISerialDevice : IAresDevice
{
  // void Connect(string portName);
  void Connect(IAresSerialPort aresSerialPort, string portName);
  void Disconnect();
}
