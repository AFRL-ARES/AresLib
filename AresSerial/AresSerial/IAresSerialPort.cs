using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal interface IAresSerialPort
{
  string? Name { get; }
  bool IsOpen { get; }
  void Open(string portName);
  void Close(Exception? error = null);
  void Listen();
  void StopListening();
  void SendOutboundMessage(string input);
  IObservable<string> DataReceived { get; }
}
