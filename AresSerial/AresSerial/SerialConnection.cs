using System;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public class SerialConnection : ISerialConnection
  {
    private Subject<SerialCommandResponse> ResponsePublisher { get; } = new Subject<SerialCommandResponse>();
    private SerialPort Port { get; }
    private IObservable<SerialCommandResponse> Responses { get; }
    private SerialCommandRequest LatestCommandRequest { get; set; }
    private Subject<bool> ListeningSource { get; } = new Subject<bool>();
    public IObservable<bool> Listening { get; }

    public CancellationTokenSource ListenerCancellationTokenSource { get; private set; }
    public SerialConnection(SerialPort port)
    {
      Listening = ListeningSource.AsObservable();
      Responses = ResponsePublisher.AsObservable();
      Port = port;
      if (Port.ReadTimeout == default)
      {
        Port.ReadTimeout = 1000;
      }
    }

    public async Task Listen()
    {
      ListenerCancellationTokenSource?.Cancel(true);
      ListenerCancellationTokenSource = new CancellationTokenSource();
      var cancellationToken = ListenerCancellationTokenSource.Token;
      cancellationToken.ThrowIfCancellationRequested();
      try
      {
        await Listen(cancellationToken);
      }
      catch (OperationCanceledException)
      {
        Console.WriteLine($"Cancelled {GetType().Name} listener loop");
      }
      finally
      {
        ListeningSource.OnNext(false);
      }

    }

    private Task Listen(CancellationToken cancellationToken)
    {
      ListeningSource.OnNext(true);
      while (!cancellationToken.IsCancellationRequested)
      {
        // TODO: Make sure we can cancel even when we're waiting for a message (thread safety)
        try
        {
          var responseMessage = Port.ReadLine(); // Blocking call, no ReadTimeout set
          var serialCommandResponse = Deserialize(responseMessage, LatestCommandRequest);
          ResponsePublisher.OnNext(serialCommandResponse);
        }
        catch (TimeoutException)
        {
          // We expect lots of these
        }
      }
      cancellationToken.ThrowIfCancellationRequested();
      return Task.CompletedTask;
    }

    public void SendCommand(SerialCommandRequest request)
    {
      var commandStr = request.Serialize();
      Port.WriteLine(commandStr);
      LatestCommandRequest = request;
    }

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
