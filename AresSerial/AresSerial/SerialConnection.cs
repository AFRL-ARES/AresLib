using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
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
    private ConcurrentQueue<SerialCommandRequest> QueuedCommandRequests { get; } = new ConcurrentQueue<SerialCommandRequest>();

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
        SerialCommandRequest oldestRequestExpectingResponse = null;
        try
        {
          ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
          var responseMessage = Port.ReadLine(); // Blocking call, no ReadTimeout set
          var expectsResponse = false;
          while (!expectsResponse)
          {
            if (!QueuedCommandRequests.TryDequeue(out oldestRequestExpectingResponse))
            {
              throw new Exception($"Couldn't dequeue a request for received response {responseMessage}");
            }
            expectsResponse = oldestRequestExpectingResponse.ExpectsResponse;
          }
          try
          {
            ListenerStatusUpdatesSource.OnNext(ListenerStatus.Busy);
            if (oldestRequestExpectingResponse is SimSerialCommandRequest simRequest)
            {
              oldestRequestExpectingResponse = simRequest.ActualRequest;
            }
            var serialCommandResponse = Deserialize(responseMessage, oldestRequestExpectingResponse);
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
        catch (TimeoutException e)
        {
          // We expect lots of these
          while (true)
          {
            if (!QueuedCommandRequests.TryPeek(out var cleanup))
            {
              break;
            }

            if (cleanup.ExpectsResponse)
            {
              break;
            }
            QueuedCommandRequests.TryDequeue(out cleanup);
          }
        }
      }
      cancellationToken.ThrowIfCancellationRequested();
    }

    public virtual void SendCommand(SerialCommandRequest request)
    {
      if (QueuedCommandRequests.Contains(request))
      {
        if (!QueuedCommandRequests.TryPeek(out var oldestRequest))
        {
          throw new Exception($"Ridiculous exception, I just don't like ignoring Try<method> outputs");
        }

        if (oldestRequest != request)
        {
          throw new Exception("This is an exception for now, but should be handled in the lib. We need to defer sending new commands until older ones have been responded to.");
        }
        // Queue was behind and now caught up to the request
      }
      else
      {
        QueuedCommandRequests.Enqueue(request);
        // Queue doesn't know about request
        // Only the oldest if queue was originally
        var syncer = Task.Run
          (
           () =>
           {
             Thread.CurrentThread.Name = "Requester";
             SerialCommandRequest oldestRequest = null;
             while (oldestRequest != request)
             {
               if (oldestRequest != null)
               {
                 Debug.WriteLine($"Waiting to send {request}, {oldestRequest} is at top of queue");
                 Thread.Sleep(500);
               }

               if (!QueuedCommandRequests.TryPeek(out oldestRequest))
               {
                 throw new Exception($"Pretty much impossible to get here, not going to put helpful information in");
               }

               //          throw new Exception("This is an exception worth catching and handling, it indicates the queue is backlogged and not going to send the command when you expect");
             }
             // Request is top of queue, ready to send
             var commandStr = request.Serialize();
             Port.WriteLine(commandStr);
           }
          );
      }
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
