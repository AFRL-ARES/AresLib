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
    private ISubject<SerialCommandResponse> ResponsePublisher { get; } = new Subject<SerialCommandResponse>();
    private IAresSerialPort Port { get; }
    private ISubject<string> PortResponsePublisher { get; } = new Subject<string>();
    private object SenderLock { get; } = new object();

    public SerialConnection(IAresSerialPort port)
    {
      Port = port;
      ConnectionStatusUpdates = ConnectionStatusUpdatesSource.AsObservable();
      Responses = ResponsePublisher.AsObservable();

      ListenerStatusUpdates = ListenerStatusUpdatesSource.AsObservable();
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Unattempted);
    }

    public void Connect()
    {
      try
      {
        Port.Open();
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

    public void StartListening()
    {
      ListenerCancellationTokenSource?.Cancel(true);
      ListenerCancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = ListenerCancellationTokenSource.Token;
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        Task.Factory.StartNew(_ => Listen(cancellationToken), cancellationToken, TaskCreationOptions.LongRunning);
      }
      catch (OperationCanceledException)
      {
        Console.WriteLine($"Cancelled {GetType().Name} listener loop");
        ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
      }
    }

    private Task Listen(CancellationToken cancellationToken)
    {
      Thread.CurrentThread.Name = $"{Port.PortName} Listener";
      ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          var response = Port.ReadLine();
          PortResponsePublisher.OnNext(response);
        }
        catch (TimeoutException)
        {
          // Timeouts are expected at a regular interval as to allow for handling cancellation
          cancellationToken.ThrowIfCancellationRequested();

        }
      }
      return Task.CompletedTask;
    }

    public virtual async Task SendCommand(SerialCommandRequest request)
    {
      var deviceString = request.Serialize();
      Task<string> syncer = null;
      if (request.ExpectsResponse)
      {
        syncer = PortResponsePublisher.Take(1)
                                      .ToTask();
      }

      lock (SenderLock)
      {
        Port.WriteLine(deviceString);
      }

      if (syncer != null)
      {
        var latestPortResponse = await syncer;
        await Task.Run
          (
           () =>
           {
             Thread.CurrentThread.Name = $"{Port.PortName} Deserializer";
             if (request is SimSerialCommandRequest simRequest)
             {
               request = simRequest.ActualRequest;
             }

             var response = Deserialize(latestPortResponse, request);
             ResponsePublisher.OnNext(response);
           }
          );
      }
    }

    public abstract SerialCommandRequest GenerateValidationRequest();

    protected virtual SerialCommandResponse Deserialize(string source, SerialCommandRequest request)
    {
      Console.WriteLine($"Unimplemented {GetType().Name} {Port.PortName} does not have implementation for deserializing response: {source} for request of type {request.GetType()}");
      return new SerialCommandResponse(source, request);
    }

    public CancellationTokenSource ListenerCancellationTokenSource { get; private set; }
    public IObservable<SerialCommandResponse> Responses { get; }
    public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    public IObservable<ListenerStatus> ListenerStatusUpdates { get; }
  }
}
