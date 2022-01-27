using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal class AresSimPort : IAresSerialPort
{

  public AresSimPort()
  {
    OutboundMessages = OutboundMessagePublisher.AsObservable();
    InboundMessages = InboundMessagePublisher.AsObservable();
  }

  private ISubject<string> OutboundMessagePublisher { get; set; } = new Subject<string>();
  private ISubject<string> InboundMessagePublisher { get; set; } = new Subject<string>();
  private Channel<string> SimulatedInboundMessages { get; set; } = Channel.CreateBounded<string>(1);

  public string Name { get; set; }

  public bool IsOpen { get; private set; }

  public IObservable<string> OutboundMessages { get; private set; }

  public IObservable<string> InboundMessages { get; private set; }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen)
      throw new Exception($"{Name} not open. Cannot send outbound message");

    var simIo = SimSerialCommandRequest<SerialCommandResponse>.GetSimulatedIo(input);
    OutboundMessagePublisher.OnNext(simIo[0]);
    if (simIo[1] == null)
      // response not expected
      return;

    var fakeInput = simIo[1];
    var random = new Random();
    Task.Delay(TimeSpan.FromMilliseconds(random.Next(100, 500))).ContinueWith(_ => SimulatedInboundMessages.Writer.TryWrite(fakeInput));
  }

  public void Open(string portName)
  {
    Name = portName;
    IsOpen = Name != null;

    OutboundMessagePublisher = new Subject<string>();
    InboundMessagePublisher = new Subject<string>();
    SimulatedInboundMessages = Channel.CreateBounded<string>(1);

    OutboundMessages = OutboundMessagePublisher.AsObservable();
    InboundMessages = InboundMessagePublisher.AsObservable();
  }

  public void Close(Exception error = null)
  {
    if (error == null)
    {
      OutboundMessagePublisher.OnCompleted();
      InboundMessagePublisher.OnCompleted();
      SimulatedInboundMessages.Writer.Complete();
    }
    else
    {
      OutboundMessagePublisher.OnError(error);
      InboundMessagePublisher.OnError(error);
      SimulatedInboundMessages.Writer.TryWrite(error.Message);// possibly not needed?
    }

    Name = null;
    IsOpen = false;
  }

  public Task ListenForEntryAsync(CancellationToken cancellationToken)
  {
    return Task.Run(
      async () => {
        Thread.CurrentThread.Name ??= $"{Name} Inbound Message";
        while (!cancellationToken.IsCancellationRequested)
        {
          if (!IsOpen)
            throw new Exception($"{Name} is not open. Cannot listen for entry");

          try
          {
            var reader = SimulatedInboundMessages.Reader.WaitToReadAsync(cancellationToken).AsTask();
            // To keep it similar to actual hardware logic
            var completedTask = await Task.WhenAny(reader, Task.Delay(1000, cancellationToken));
            if (completedTask != reader)
              throw new TimeoutException($"SimPort {nameof(ListenForEntryAsync)} timed out");

            var result = await SimulatedInboundMessages.Reader.ReadAsync(cancellationToken);
            InboundMessagePublisher.OnNext(result);
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
