using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Ares.Device.Serial.Simulation;

namespace Ares.Device.Serial;

public abstract class SerialConnection : ISerialConnection
{
  private readonly object _lock = new();

  protected SerialConnection(SerialPortConnectionInfo connectionInfo) : this(new AresHardwarePort(connectionInfo))
  {
  }

  protected SerialConnection(SimAresDevice simDevice) : this(new AresSimPort(simDevice.OutputChannel, simDevice.InputChannel))
  {
  }

  private SerialConnection(IAresSerialPort port)
  {
    Port = port;
    ConnectionStatusUpdates = ConnectionStatusUpdatesSource.AsObservable();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Unattempted);
  }

  private ISubject<ConnectionStatus> ConnectionStatusUpdatesSource { get; } = new BehaviorSubject<ConnectionStatus>(ConnectionStatus.Unattempted);
  private IAresSerialPort Port { get; }

  public void Connect(string portName)
  {
    try
    {
      Port.Open(portName);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Failed);
      throw;
    }

    if (!Port.IsOpen)
    {
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Failed);
      return;
    }

    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Connected);
  }

  public void Disconnect()
  {
    Port.Close();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.ManuallyClosed);
  }

  public virtual void Send(SerialCommandRequest request)
  {
    lock ( _lock )
    {
      Port.SendOutboundMessage(request.Serialize());
    }
  }

  public Task<T> SendAndGetResponseAsync<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken, TimeSpan timeout) where T : SerialCommandResponse
  {
    return Task.Run(() => SendAndGetResponse(request, cancellationToken, timeout), cancellationToken);
  }

  public Task<T> SendAndGetResponseAsync<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken) where T : SerialCommandResponse
  {
    return Task.Run(() => SendAndGetResponse(request, cancellationToken), cancellationToken);
  }

  public virtual T SendAndGetResponse<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken, TimeSpan timeout) where T : SerialCommandResponse
  {
    lock ( _lock )
    {
      var transactionSyncer = Port.ListenForEntryAsync(cancellationToken, timeout);
      var outboundMessage = request.Serialize();

      Port.SendOutboundMessage(outboundMessage);
      transactionSyncer.Wait(cancellationToken);
      var latestPortResponse = transactionSyncer.Result;
      var response = request.DeserializeResponse(latestPortResponse);
      return response;
    }
  }

  public T SendAndGetResponse<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken) where T : SerialCommandResponse
    => SendAndGetResponse(request, cancellationToken, Timeout.InfiniteTimeSpan);

  public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
}
