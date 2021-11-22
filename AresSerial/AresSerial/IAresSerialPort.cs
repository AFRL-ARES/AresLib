using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface IAresSerialPort
  {
    string Name { get; }
    bool IsOpen { get; }
    IObservable<string> OutboundMessages { get; }
    IObservable<string> InboundMessages { get; }

    void Open();
    Task ListenForEntryAsync(CancellationToken cancellationToken);
    void SendOutboundMessage(string input);
  }
}
