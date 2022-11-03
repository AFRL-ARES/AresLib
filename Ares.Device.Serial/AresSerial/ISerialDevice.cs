using System;

namespace Ares.Device.Serial;

public interface ISerialDevice<TConnection> : IAresDevice where TConnection : IAresSerialPort
{
  void Connect(TConnection aresSerialPort, string portName);
  void Disconnect();
}
