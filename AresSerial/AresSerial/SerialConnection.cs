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
#if SIM
    private bool _simBlock = true;
#endif
    private ISubject<ConnectionResult> StatusUpdatesSource { get; } = new BehaviorSubject<ConnectionResult>(ConnectionResult.Unattempted);
    private ISubject<SerialCommandResponse> ResponsePublisher { get; } = new Subject<SerialCommandResponse>();
    private SerialPort Port { get; }
    protected IObservable<SerialCommandResponse> Responses { get; }
    private SerialCommandRequest LatestCommandRequest { get; set; }

    public CancellationTokenSource ListenerCancellationTokenSource { get; private set; }

    public IObservable<ConnectionResult> StatusUpdates { get; }

    public SerialConnection(SerialPort port)
    {
      StatusUpdates = StatusUpdatesSource.AsObservable();
      Responses = ResponsePublisher.AsObservable();
      Port = port;
      if (Port.ReadTimeout == default)
      {
        Port.ReadTimeout = 1000;
      }
      StatusUpdatesSource.OnNext(ConnectionResult.Unattempted);
    }

    public void Connect()
    {
#if SIM
      StatusUpdatesSource.OnNext(ConnectionResult.Connected);
      return;
#endif
      try
      {
        Port.Open();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        StatusUpdatesSource.OnNext(ConnectionResult.FailedConnection);
        throw;
      }

      if (!Port.IsOpen)
      {
        StatusUpdatesSource.OnNext(ConnectionResult.FailedConnection);
        return;
      }

      StatusUpdatesSource.OnNext(ConnectionResult.Connected);
    }

    public async Task Listen()
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
        StatusUpdatesSource.OnNext(ConnectionResult.ListenerPaused);
      }

    }

    private Task Listen(CancellationToken cancellationToken)
    {
      Thread.CurrentThread.Name = $"{GetType().Name}";
      while (!cancellationToken.IsCancellationRequested)
      {
        // TODO: Make sure we can cancel even when we're waiting for a message (thread safety)
        try
        {
          StatusUpdatesSource.OnNext(ConnectionResult.Listening);
#if SIM
          if (_simBlock)
          {
            Thread.Sleep(1000);
            continue;
          }
          var responseMessage = "SIM";
#else
          var responseMessage = Port.ReadLine(); // Blocking call, no ReadTimeout set
#endif
          try
          {
            var serialCommandResponse = Deserialize(responseMessage, LatestCommandRequest);
            if (serialCommandResponse == null)
            {
              throw new NullReferenceException();
            }
            StatusUpdatesSource.OnNext(ConnectionResult.ValidResponse);
            ResponsePublisher.OnNext(serialCommandResponse);
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
            StatusUpdatesSource.OnNext(ConnectionResult.InvalidResponse);
            throw;
          }
        }
        catch (TimeoutException)
        {
          // We expect lots of these
        }
#if SIM
        _simBlock = true;
#endif
      }
      cancellationToken.ThrowIfCancellationRequested();
      return Task.CompletedTask;
    }

    public void SendCommand(SerialCommandRequest request)
    {
      var commandStr = request.Serialize();
#if !SIM
      Port.WriteLine(commandStr);
#endif
      LatestCommandRequest = request;
#if SIM
      _simBlock = false;
#endif
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
  }
}
