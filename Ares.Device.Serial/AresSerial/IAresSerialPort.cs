using System;
using System.Threading.Tasks;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial;

public interface IAresSerialPort
{
  string? Name { get; }
  bool IsOpen { get; }
  void AttemptOpen(string portName);
  void Listen();
  Task<T> Send<T>(SerialCommandWithResponse<T> command) where T : SerialResponse;
  void PersistOutboundCommand<T>(SerialCommandWithResponse<T> command) where T : SerialResponse;
  IObservable<SerialTransaction<T>> GetTransactionStream<T>() where T : SerialResponse;
  void Send(SerialCommand command);

  /// <summary>
  /// Closes the underlying port and stops the internal message listener
  /// </summary>
  void Close();

  void StopListening();
}
