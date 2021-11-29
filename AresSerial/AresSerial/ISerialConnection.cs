using System;

namespace AresSerial
{
  public interface ISerialConnection
  {
    IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    IObservable<ListenerStatus> ListenerStatusUpdates { get; }
    IObservable<SerialCommandResponse> Responses { get; }
    void Connect(string portName);
    void Disconnect();
    void StartListening();
    void StopListening();
    void SendAndWaitForReceipt(SerialCommandRequest request);
    SerialCommandRequest GenerateValidationRequest();
  }
}
