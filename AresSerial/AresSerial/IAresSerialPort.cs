using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal interface IAresSerialPort
{
  string? Name { get; }
  bool IsOpen { get; }
  void Open(string portName);
  void Close(Exception? error = null);
  Task<string> ListenForEntryAsync(CancellationToken cancellationToken, TimeSpan timeout = default);
  void SendOutboundMessage(string input);
}
