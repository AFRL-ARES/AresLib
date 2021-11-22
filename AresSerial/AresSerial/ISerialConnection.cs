using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface ISerialConnection
  {
    CancellationTokenSource ListenerCancellationTokenSource { get; }
    IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    IObservable<ListenerStatus> ListenerStatusUpdates { get; }
    IObservable<SerialCommandResponse> Responses { get; }
    void Connect();
    void StartListening();
    void StopListening();
    void SendAndWaitForReceipt(SerialCommandRequest request);
    SerialCommandRequest GenerateValidationRequest();
  }
}
