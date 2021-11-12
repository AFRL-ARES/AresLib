using System;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    private SerialCommandRequest LatestCommandRequest { get; set; }

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

    public async void StartListening()
    {
      ListenerCancellationTokenSource?.Cancel(true);
      ListenerCancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = ListenerCancellationTokenSource.Token;
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        await Task.Run(() => Listen(cancellationToken), cancellationToken);
      }
      catch (OperationCanceledException)
      {
        Console.WriteLine($"Cancelled {GetType().Name} listener loop");
      }
      catch (Exception e)
      {
        Console.WriteLine($"{GetType().Name} listener on thread {Thread.CurrentThread.Name} encountered an error: {e}");
        throw;
      }
      finally
      {
        ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
      }
    }

    private void Listen(CancellationToken cancellationToken)
    {
      Thread.CurrentThread.Name = $"{GetType().Name} Listener";
      while (!cancellationToken.IsCancellationRequested)
      {
        // TODO: Make sure we can cancel even when we're waiting for a message (thread safety)
        try
        {
          ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
          var responseMessage = Port.ReadLine(); // Blocking call, no ReadTimeout set

          try
          {
            ListenerStatusUpdatesSource.OnNext(ListenerStatus.Busy);
            var serialCommandResponse = Deserialize(responseMessage, LatestCommandRequest);
            if (serialCommandResponse == null)
            {
              throw new NullReferenceException();
            }
            ResponsePublisher.OnNext(serialCommandResponse);
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
            ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
            throw;
          }
        }
        catch (TimeoutException)
        {
          // We expect lots of these
        }
      }
      cancellationToken.ThrowIfCancellationRequested();
    }

    public void SendCommand(SerialCommandRequest request)
    {
      LatestCommandRequest = request;
      var commandStr = request.Serialize();
      Port.WriteLine(commandStr);
    }

    public abstract SerialCommandRequest GenerateValidationRequest();

    protected virtual void ProcessResponse(SerialCommandResponse commandResponse)
    {
      if (GetType() == typeof(SerialConnection))
      {
        Console.WriteLine($"{Port.PortName} Received: {commandResponse.Message} <--- {commandResponse.SourceRequest.Serialize()}");
      }
    }

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
