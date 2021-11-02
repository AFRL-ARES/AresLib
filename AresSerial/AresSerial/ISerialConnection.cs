using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface ISerialConnection
  {
    CancellationTokenSource ListenerCancellationTokenSource { get; }
    IObservable<bool> Listening { get; }
    Task Listen();
    void SendCommand(SerialCommandRequest request);
  }
}
