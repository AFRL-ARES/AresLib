using Ares.Device.Serial.Simulation;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

public interface IAresSerialPort
{
  string? Name { get; }
  bool IsOpen { get; }
  void AttemptOpen(string portName);
  void Listen();
  void SendOutboundMessage(byte[] input);
  
  /// <summary>
  /// Closes the underlying port and stops the internal message listener
  /// </summary>
  void Disconnect();
  void StopListening();
}
