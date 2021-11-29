using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  internal class AresSimPort : IAresSerialPort
  {
    private ISubject<string> OutboundMessagePublisher { get; set; } = new Subject<string>();
    private ISubject<string> InboundMessagePublisher { get; set; } // Mayyyybe don't need this if it isn't going to be exposed at the base level
    private ISubject<string> ExpectedInboundMessages { get; set; } = new Subject<string>();

    public AresSimPort()
    {
      OutboundMessages = OutboundMessagePublisher.AsObservable();
      InboundMessagePublisher = ExpectedInboundMessages;
      InboundMessages = ExpectedInboundMessages.AsObservable();
    }

    public string Name { get; set; }

    public bool IsOpen { get; private set; }

    public IObservable<string> OutboundMessages { get; private set; }

    public IObservable<string> InboundMessages { get; private set; }

    public void SendOutboundMessage(string input)
    {
      if (!IsOpen)
      {
        throw new Exception($"{Name} not open. Cannot send outbound message");
      }
      var simIo = SimSerialCommandRequest.GetSimulatedIo(input);
      OutboundMessagePublisher.OnNext(simIo[0]);
      if (simIo[1] == null)
      {
        // response not expected
        return;
      }

      var fakeInput = simIo[1];
      ExpectedInboundMessages.OnNext(fakeInput);
    }

    public void Open(string portName)
    {
      Name = portName;
      IsOpen = Name != null;

      OutboundMessagePublisher = new Subject<string>();
      ExpectedInboundMessages = new Subject<string>();
      InboundMessagePublisher = ExpectedInboundMessages;

      OutboundMessages = OutboundMessagePublisher.AsObservable();
      InboundMessages = ExpectedInboundMessages.AsObservable();
    }

    public void Close(Exception error = null)
    {
      if (error == null)
      {
        OutboundMessagePublisher.OnCompleted();
        InboundMessagePublisher.OnCompleted();
        ExpectedInboundMessages.OnCompleted();
      }
      else
      {
        OutboundMessagePublisher.OnError(error);
        InboundMessagePublisher.OnError(error);
        ExpectedInboundMessages.OnError(error);
      }

      Name = null;
      IsOpen = false;
    }

    public Task ListenForEntryAsync(CancellationToken cancellationToken)
    {
      return Task.Run
        (
         async () =>
         {
           Thread.CurrentThread.Name ??= $"{Name} Inbound Message";
           var reader = InboundMessages.Take(1)
                                               .ToTask(cancellationToken);
           while (!cancellationToken.IsCancellationRequested)
           {
             if (!IsOpen)
             {
               throw new Exception($"{Name} is not open. Cannot listen for entry");
             }
             try
             {
               // To keep it similar to actual hardware logic
               var completedTask = await Task.WhenAny(reader, Task.Delay(1000));
               if (completedTask != reader)
               {
                 throw new TimeoutException($"SimPort {nameof(ListenForEntryAsync)} timed out");
               }
               InboundMessagePublisher.OnNext(reader.Result);
             }
             catch (Exception)
             {
               cancellationToken.ThrowIfCancellationRequested();
             }
             break;
           }

         },
         cancellationToken
        );
    }
  }
}
