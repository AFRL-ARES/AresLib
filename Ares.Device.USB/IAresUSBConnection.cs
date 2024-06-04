using Ares.Device.USB.Commands;

namespace Ares.Device.Serial;
public interface IAresUSBConnection : IDisposable
{
  string? Name { get; }
  bool IsOpen { get; }
  void AttemptOpen();
  Task<T> Send<T>(USBCommandWithResponse<T> command) where T : USBResponse;
  Task<T> Send<T>(USBCommandWithResponse<T> command, TimeSpan timeout) where T : USBResponse;
  Task<T> Send<T>(USBCommandWithResponse<T> command, TimeSpan timeout, Func<T, bool> filter) where T : USBResponse;
  Task<T> Send<T>(USBCommandWithResponse<T> command, Func<T, bool> filter) where T : USBResponse;
  Task Send(USBCommand command);
  void Close();
}
