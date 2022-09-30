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
  /// Tries to open the underlying serial port and updates the <see cref="ConnectionStatusUpdates" />
  /// </summary>
  /// <param name="portName">Name of the port to open</param>
  void Connect(string portName);

  /// <summary>
  /// Closes the underlying port and stops the internal message listener
  /// </summary>
  void Disconnect();

  /// <summary>
  /// Sends a request over the serial port and blocks the calling thread until a response
  /// is received, the operation times out due to
  /// <param name="timeout" />
  /// , or the
  /// <param name="cancellationToken" />
  /// is canceled
  /// Throws a <see cref="TimeoutException" /> if getting the response takes longer than
  /// the specified timeout.
  /// Note: Calling this from multiple threads does not guarantee order in which the requests are processed
  /// </summary>
  /// <param name="request">The command to be sent to the port</param>
  /// <param name="cancellationToken"></param>
  /// <param name="timeout">How long to wait before timing out on receipt</param>
  /// <returns>The response for the given request</returns>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  T SendAndGetResponse<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken, TimeSpan timeout) where T : SerialCommandResponse;

  /// <summary>
  /// Sends a request over the serial port and blocks the calling thread until a response
  /// is received or the
  /// <param name="cancellationToken" />
  /// is canceled
  /// Note: Calling this from multiple threads does not guarantee order in which the requests are processed
  /// </summary>
  /// <param name="request">The command to be sent to the port</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The response for the given request</returns>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  T SendAndGetResponse<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken) where T : SerialCommandResponse;

  /// <summary>
  /// Creates a task that sends a request over the serial port and completes when a response
  /// is received, the operation times out due to
  /// <param name="timeout" />
  /// , or the
  /// <param name="cancellationToken" />
  /// is canceled
  /// Throws a <see cref="TimeoutException" /> if getting the response takes longer than
  /// the specified timeout.
  /// Note: Calling this from multiple threads does not guarantee order in which the requests are processed
  /// </summary>
  /// <param name="request">The command to be sent to the port</param>
  /// <param name="cancellationToken"></param>
  /// <param name="timeout">How long to wait before timing out on receipt</param>
  /// <returns>The response for the given request</returns>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  Task<T> SendAndGetResponseAsync<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken, TimeSpan timeout) where T : SerialCommandResponse;

  /// <summary>
  /// Creates a task that sends a request over the serial port and completes when a response
  /// is received or the
  /// <param name="cancellationToken" />
  /// is canceled
  /// Throws a <see cref="TimeoutException" /> if getting the response takes longer than
  /// the specified timeout.
  /// Note: Calling this from multiple threads does not guarantee order in which the requests are processed
  /// </summary>
  /// <param name="request">The command to be sent to the port</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The response for the given request</returns>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  Task<T> SendAndGetResponseAsync<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken) where T : SerialCommandResponse;

  /// <summary>
  /// Sends a request over the serial port without expecting a response
  /// </summary>
  /// <param name="request"></param>
  void Send(SerialCommandRequest request);
}
