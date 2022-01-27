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
  /// Listens internally for a response from the underlying serial port
  /// </summary>
  /// <typeparam name="T">The type of <see cref="SerialCommandResponse" /> to listen for</typeparam>
  /// <param name="cancelToken">Cancellation token to cancel this particular listener</param>
  /// <param name="timeout">How long to wait for the response before timing out</param>
  /// <returns>An awaitable <see cref="Task" /> that returns a requested <see cref="SerialCommandResponse" /></returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown when this method is called but <see cref="ListenerStatusUpdates" />
  /// is not <see cref="ListenerStatus.Listening" />
  /// </exception>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  Task<T> GetResponse<T>(CancellationToken cancelToken, TimeSpan timeout) where T : SerialCommandResponse;

  /// <summary>
  /// Listens internally for any response from the underlying serial port
  /// </summary>
  /// <param name="cancelToken">Cancellation token to cancel this particular listener</param>
  /// <param name="timeout">How long to wait for the response before timing out</param>
  /// <returns>
  /// An awaitable <see cref="Task" /> that returns the first <see cref="SerialCommandResponse" /> to arrive on the
  /// serial port
  /// </returns>
  /// <exception cref="InvalidOperationException">Thrown when this method is called but the internal listener is not running</exception>
  /// <exception cref="TimeoutException">Timed out before getting a response</exception>
  Task<SerialCommandResponse> GetAnyResponse(CancellationToken cancelToken, TimeSpan timeout);

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
  /// Stops the internal listener
  /// </summary>
  void StopListening();

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
