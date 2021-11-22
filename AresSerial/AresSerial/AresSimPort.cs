using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public class AresSimPort : IAresSerialPort
  {
    private ISubject<string> OutboundMessagePublisher { get; } = new Subject<string>();
    private ISubject<string> InboundMessagePublisher { get; } // Mayyyybe don't need this if it isn't going to be exposed at the base level
    private ISubject<string> ExpectedInboundMessages { get; } = new Subject<string>();

    public AresSimPort(string name)
    {
      Name = name;
      OutboundMessages = OutboundMessagePublisher.AsObservable();
      InboundMessagePublisher = ExpectedInboundMessages;
      InboundMessages = ExpectedInboundMessages.AsObservable();
    }

    public string Name { get; set; }

    public bool IsOpen { get; private set; }

    public IObservable<string> OutboundMessages { get; }

    public IObservable<string> InboundMessages { get; }

    public virtual void Open()
    {
      IsOpen = true;
    }

    public void SendOutboundMessage(string input)
    {
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
             catch (Exception e)
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
