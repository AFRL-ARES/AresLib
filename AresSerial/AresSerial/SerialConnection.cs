using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public abstract class SerialConnection : ISerialConnection
  {
    private ISubject<ConnectionStatus> ConnectionStatusUpdatesSource { get; } = new BehaviorSubject<ConnectionStatus>(ConnectionStatus.Unattempted);
    private ISubject<ListenerStatus> ListenerStatusUpdatesSource { get; } = new BehaviorSubject<ListenerStatus>(ListenerStatus.Idle);
    private ISubject<SerialCommandResponse> ResponsePublisher { get; } = Subject.Synchronize(new Subject<SerialCommandResponse>());
    private IAresSerialPort Port { get; }
    private ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

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
      var alreadyListening = ListenerCancellationTokenSource?.IsCancellationRequested ?? false;
      if (!alreadyListening)
      {
        ListenerCancellationTokenSource = new CancellationTokenSource();
      }
      else
      {
        throw new Exception($"{Port.Name} Listener loop already started");
      }

      try
      {
        Task.Run(() => ListenAsync(ListenerCancellationTokenSource.Token), ListenerCancellationTokenSource.Token);
      }
      catch (OperationCanceledException)
      {
        Console.WriteLine($"Cancelled {GetType().Name} listener loop");
        ListenerStatusUpdatesSource.OnNext(ListenerStatus.Paused);
      }
    }

    public void StopListening()
    {
      ListenerCancellationTokenSource.Cancel();
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
      Thread.CurrentThread.Name = $"{Port.Name} Listener Loop";
      ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
      while (!cancellationToken.IsCancellationRequested)
      {
          await Port.ListenForEntryAsync(cancellationToken);
      }
    }

    public virtual void SendAndWaitForReceipt(SerialCommandRequest request)
    {
        if (Thread.CurrentThread.Name == null)
        {
          Thread.CurrentThread.Name = $"{Port.Name} Transaction";
        }

        var transactionSyncer = Port.InboundMessages.Take(1)
                                                     .ToTask();
        var outboundMessage = request.Serialize();

        Lock.EnterWriteLock();
        Port.SendOutboundMessage(outboundMessage);
        if (!request.ExpectsResponse)
        {
          Lock.ExitWriteLock();
          return;
      }
        Lock.ExitWriteLock();

      var latestPortResponse = transactionSyncer.Result;

        Task.Run
          (
           () =>
           {
             Thread.CurrentThread.Name = $"{Port.Name} Deserializer";
             if (request is SimSerialCommandRequest simRequest)
             {
               request = simRequest.ActualRequest;
             }

             var response = Deserialize(latestPortResponse, request);
             ResponsePublisher.OnNext(response);
           }
          );
    }



    public abstract SerialCommandRequest GenerateValidationRequest();

    protected virtual SerialCommandResponse Deserialize(string source, SerialCommandRequest request)
    {
      Console.WriteLine($"Unimplemented {GetType().Name} {Port.Name} does not have implementation for deserializing response: {source} for request of type {request.GetType()}");
      return new SerialCommandResponse(source, request);
    }

    public CancellationTokenSource ListenerCancellationTokenSource { get; private set; }
    public IObservable<SerialCommandResponse> Responses { get; }
    public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
    public IObservable<ListenerStatus> ListenerStatusUpdates { get; }
  }
}
