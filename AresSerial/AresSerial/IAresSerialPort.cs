using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  internal interface IAresSerialPort
  {
    void Open(string portName);
    void Close(Exception error = null);
    Task ListenForEntryAsync(CancellationToken cancellationToken);
    void SendOutboundMessage(string input);
    string Name { get; }
    bool IsOpen { get; }
    IObservable<string> OutboundMessages { get; }
    IObservable<string> InboundMessages { get; }
  }
}
