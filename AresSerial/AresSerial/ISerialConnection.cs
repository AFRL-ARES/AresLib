using System;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public interface ISerialConnection
  {
    IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    IObservable<ListenerStatus> ListenerStatusUpdates { get; }
    Task<T> GetResponse<T>(CancellationToken cancelToken);
    Task<SerialCommandResponse> GetAnyResponse(CancellationToken cancelToken);
    void Connect(string portName);
    void Disconnect();
    void StartListening();
    void StopListening();
    void SendAndWaitForReceipt(SerialCommandRequest request);
    SerialCommandRequest GenerateValidationRequest();
  }
}
