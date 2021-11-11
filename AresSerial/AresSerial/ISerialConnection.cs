using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface ISerialConnection
  {
    CancellationTokenSource ListenerCancellationTokenSource { get; }
    IObservable<ConnectionResult> StatusUpdates { get; }
    void Connect();
    Task Listen();
    void SendCommand(SerialCommandRequest request);
    SerialCommandRequest GenerateValidationRequest();
  }
}
