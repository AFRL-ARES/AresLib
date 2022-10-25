using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

/// <summary>
/// Represents a bridge between an <see cref="ISerialDevice" /> and a hardware serial port
/// Provides an abstracted way to send and receive data over the serial port
/// </summary>
public interface ISerialConnection
{
  /// <summary>
  /// An observable which reports the <see cref="ConnectionStatus" /> of the connection
  /// </summary>
  IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }

  /// <summary>
  /// An observable indicating the <see cref="ListenerStatus" /> of the message listener
  /// </summary>
  IObservable<ListenerStatus> ListenerStatusUpdates { get; }
 
  /// <summary>
  /// Tries to open the underlying serial port and updates the <see cref="ConnectionStatusUpdates" />
  /// </summary>
  /// <param name="portName">Name of the port to open</param>
  void Connect(string portName);

  /// <summary>
  /// Closes the underlying port and stops the internal message listener
  /// </summary>
  void Disconnect();

  /// <summary>
  /// Starts an internal listener that listens for any messages arriving on the underlying serial port
  /// </summary>
  void StartListening();

  /// <summary>
  /// Stops the internal listener, preventing buffer updates
  /// </summary>
  void StopListening();


  /// <summary>
  /// Sends a request over the serial port without waiting for a response
  /// </summary>
  /// <param name="request"></param>
  void Send(string request);

  /// <summary>
  /// Sends a request over the serial port without waiting for a response
  /// </summary>
  /// <param name="request"></param>
  void Send(byte[] request);
}
