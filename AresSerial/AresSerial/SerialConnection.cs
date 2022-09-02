using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
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
    Responses = ResponsePublisher.AsObservable();

    ListenerStatusUpdates = ListenerStatusUpdatesSource.AsObservable();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Unattempted);
  }

  private ISubject<ConnectionStatus> ConnectionStatusUpdatesSource { get; } = new BehaviorSubject<ConnectionStatus>(ConnectionStatus.Unattempted);
  private ISubject<ListenerStatus> ListenerStatusUpdatesSource { get; } = new BehaviorSubject<ListenerStatus>(ListenerStatus.Idle);
  private ISubject<SerialCommandResponse> ResponsePublisher { get; set; } = new Subject<SerialCommandResponse>();
  private IAresSerialPort Port { get; }
  private IObservable<SerialCommandResponse> Responses { get; set; }
  private CancellationTokenSource? ListenerCancellationTokenSource { get; set; }

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
    StopListening();
    Port.Close();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.ManuallyClosed);
  }

  public void StartListening()
  {
    var alreadyListening = !ListenerCancellationTokenSource?.Token.IsCancellationRequested ?? false;
    if (!alreadyListening)
    {
      ResponsePublisher = new Subject<SerialCommandResponse>();
      Responses = ResponsePublisher.AsObservable();
      ListenerCancellationTokenSource = new CancellationTokenSource();
    }
    else
    {
      throw new InvalidOperationException($"{Port.Name} Listener loop already started");
    }

    var listenThread = new Thread(() => ListenAsync(ListenerCancellationTokenSource.Token));
    listenThread.Start();
  }

  public void StopListening()
  {
    ListenerCancellationTokenSource?.Cancel();
    ResponsePublisher.OnCompleted();
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
      var transactionSyncer = Port.InboundMessages.Take(1)
        .ToTask(cancellationToken);

      var timeoutCancel = new CancellationTokenSource();
      var aggregateCancelSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancel.Token);
      var timeoutSyncer = Task.Delay(timeout, aggregateCancelSource.Token);
      var outboundMessage = request.Serialize();

      Port.SendOutboundMessage(outboundMessage);

      var finishedTask = Task.WhenAny(transactionSyncer, timeoutSyncer).Result;

      if (finishedTask == timeoutSyncer)
        throw new TimeoutException($"Could not get a receipt for command \"{request.Serialize()}\" on port {Port.Name} within {timeout}");

      timeoutCancel.Cancel();
      var latestPortResponse = transactionSyncer.Result;

      if (request is SimRequestWithResponse<T> simRequest)
        request = simRequest.ActualRequest;

      var response = request.DeserializeResponse(latestPortResponse);
      ResponsePublisher.OnNext(response);
      return response;
    }
  }

  public T SendAndGetResponse<T>(SerialCommandRequestWithResponse<T> request, CancellationToken cancellationToken) where T : SerialCommandResponse
    => SendAndGetResponse(request, cancellationToken, Timeout.InfiniteTimeSpan);

  public Task<T> GetResponse<T>(CancellationToken cancellationToken, TimeSpan timeout) where T : SerialCommandResponse
  {
    var listenerStatus = ListenerStatusUpdates.FirstAsync().Wait();
    if (listenerStatus != ListenerStatus.Listening)
      throw new InvalidOperationException($"{Port.Name} Connection attempted to get a response while the listener is {listenerStatus}");

    return Responses.OfType<T>().FirstAsync().Timeout(timeout).ToTask(cancellationToken);
  }

  public Task<SerialCommandResponse> GetAnyResponse(CancellationToken cancellationToken, TimeSpan timeout)
    => GetResponse<SerialCommandResponse>(cancellationToken, timeout);

  public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
  public IObservable<ListenerStatus> ListenerStatusUpdates { get; }

  private async void ListenAsync(CancellationToken cancellationToken)
  {
    Thread.CurrentThread.Name ??= $"{Port.Name} Listener Loop";
    Thread.CurrentThread.IsBackground = true;
    ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
    while (!cancellationToken.IsCancellationRequested)
      try
      {
        await Port.ListenForEntryAsync(cancellationToken);
      }

      catch (OperationCanceledException)
      {
        Console.WriteLine($"Canceled {GetType().Name} listener loop");
        ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
      }
  }
}
