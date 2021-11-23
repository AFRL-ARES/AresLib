using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface ISerialConnection
  {
    IAresSerialPort Port { get; }
    CancellationTokenSource ListenerCancellationTokenSource { get; }
    IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    IObservable<ListenerStatus> ListenerStatusUpdates { get; }
    IObservable<SerialCommandResponse> Responses { get; }
    void Connect(string portName);
    void Disconnect();
    void StartListening();
    void SendAndWaitForReceipt(SerialCommandRequest request);
    SerialCommandRequest GenerateValidationRequest();
  }
}
