namespace Ares.Device.Serial;
public interface IAresUSBConnection : IAresDeviceConnection
{
  void AttemptOpen();
}
