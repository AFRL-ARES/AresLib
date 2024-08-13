namespace Ares.Device.Serial;
public interface IAresUSBConnection : IAresDeviceConnection
{
  bool IsOpen { get; }
  void AttemptOpen();
}
