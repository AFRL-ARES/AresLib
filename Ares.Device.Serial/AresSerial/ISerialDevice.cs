using System.Threading.Tasks;

namespace Ares.Device.Serial;

public interface ISerialDevice<TConnection> : IAresDevice where TConnection : IAresSerialPort
{
  Task Connect(TConnection aresSerialPort, string portName);
  void Disconnect();
}
