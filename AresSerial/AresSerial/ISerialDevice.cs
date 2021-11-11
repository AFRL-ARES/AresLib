using AresDevicePluginBase;

namespace AresSerial
{
  public interface ISerialDevice<TConnection> : IAresDevice where TConnection : ISerialConnection
  {
    TConnection EstablishConnection();
  }
}
