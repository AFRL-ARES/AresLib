using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public abstract class SerialConnection : ISerialConnection
  {
    private ISubject<ConnectionStatus> ConnectionStatusUpdatesSource { get; } = new BehaviorSubject<ConnectionStatus>(ConnectionStatus.Unattempted);
    private ISubject<ListenerStatus> ListenerStatusUpdatesSource { get; } = new BehaviorSubject<ListenerStatus>(ListenerStatus.Idle);
    private ISubject<SerialCommandResponse> ResponsePublisher { get; set; } = new Subject<SerialCommandResponse>();
    private IAresSerialPort Port { get; }
    private readonly object Lock = new object();
    private IObservable<SerialCommandResponse> Responses { get; set; }
    private CancellationTokenSource ListenerCancellationTokenSource { get; set; }

    public SerialConnection(SerialPortConnectionInfo connectionInfo = null)
    {
      if (connectionInfo is null)
        Port = new AresSimPort();
      else
      {
        Port = new AresHardwarePort(connectionInfo);
      }
      ConnectionStatusUpdates = ConnectionStatusUpdatesSource.AsObservable();
      Responses = ResponsePublisher.AsObservable();

      ListenerStatusUpdates = ListenerStatusUpdatesSource.AsObservable();
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Unattempted);
    }

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
        throw new Exception($"{Port.Name} Listener loop already started");
      }

      Task.Run(() => ListenAsync(ListenerCancellationTokenSource.Token), ListenerCancellationTokenSource.Token);
    }

    public void StopListening()
    {
      ListenerCancellationTokenSource?.Cancel();
      ResponsePublisher.OnCompleted();
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
      Thread.CurrentThread.Name = $"{Port.Name} Listener Loop";
      ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          await Port.ListenForEntryAsync(cancellationToken);
        }

        catch (OperationCanceledException)
        {
          Console.WriteLine($"Cancelled {GetType().Name} listener loop");
          ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
        }
      }
    }

    public virtual void SendAndWaitForReceipt(SerialCommandRequest request)
    {
      if (Thread.CurrentThread.Name == null)
      {
        Thread.CurrentThread.Name = $"{Port.Name} Transaction";
      }

      lock (Lock)
      {
        var transactionSyncer = Port.InboundMessages.Take(1)
          .ToTask();
        var outboundMessage = request.Serialize();

        Port.SendOutboundMessage(outboundMessage);


        if (!request.ExpectsResponse)
        {
          return;
        }

        var latestPortResponse = transactionSyncer.Result;

        if (request is SimSerialCommandRequest simRequest)
        {
          request = simRequest.ActualRequest;
        }

        var response = Deserialize(latestPortResponse, request);
        ResponsePublisher.OnNext(response);
      }
    }


    public abstract SerialCommandRequest GenerateValidationRequest();

    protected virtual SerialCommandResponse Deserialize(string source, SerialCommandRequest request)
    {
      Console.WriteLine($"Unimplemented {GetType().Name} {Port.Name} does not have implementation for deserializing response: {source} for request of type {request.GetType()}");
      return new SerialCommandResponse(source, request);
    }

    public Task<T> GetResponse<T>(CancellationToken cancellationToken)
    {
      var listenerStatus = ListenerStatusUpdates.FirstAsync().Wait();
      if (listenerStatus != ListenerStatus.Listening)
        throw new Exception($"{Port.Name} Connection attempted to get a response while the listener is {listenerStatus}");
      return Responses.OfType<T>().FirstAsync().ToTask(cancellationToken);
    }

    public Task<SerialCommandResponse> GetAnyResponse(CancellationToken cancellationToken)
    {
      return GetResponse<SerialCommandResponse>(cancellationToken);
    }

    public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; private set; }
    public IObservable<ListenerStatus> ListenerStatusUpdates { get; private set; }
  }
}
