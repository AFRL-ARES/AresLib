using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal interface IAresSerialPort
{
  string Name { get; }
  bool IsOpen { get; }
  IObservable<string> OutboundMessages { get; }
  IObservable<string> InboundMessages { get; }
  void Open(string portName);
  void Close(Exception error = null);
  Task ListenForEntryAsync(CancellationToken cancellationToken);
  void SendOutboundMessage(string input);
}
