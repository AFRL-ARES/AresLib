using System;
using System.Threading.Tasks;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial;

public interface IAresSerialPort
{
  string? Name { get; }
  bool IsOpen { get; }
  void AttemptOpen(string portName);
  Task<T> Send<T>(SerialCommandWithResponse<T> command) where T : SerialResponse;
  void AttemptSendDistinct<T>(SerialCommandWithStreamedResponse<T> command) where T : SerialResponse;
  IObservable<SerialTransaction<T>> GetTransactionStream<T>() where T : SerialResponse;
  void Send(SerialCommand command);
  void Close();
}
