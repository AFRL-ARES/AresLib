namespace Ares.Device.Serial;

public interface ISerialDevice<TConnection> : IAresDevice where TConnection : IAresSerialPort
{
  // void Connect(string portName);
  void Connect(TConnection aresSerialPort, string portName);
  void Disconnect();
}
