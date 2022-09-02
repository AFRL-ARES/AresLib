using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ares.Device.Serial.Simulation;

internal class AresSimPort : IAresSerialPort
{

  public AresSimPort(Channel<string> inputChannel, Channel<string> outputChannel)
  {
    OutboundMessages = OutboundMessagePublisher.AsObservable();
    InboundMessages = InboundMessagePublisher.AsObservable();
    InputChannel = inputChannel;
    OutputChannel = outputChannel;
  }

  private ISubject<string> OutboundMessagePublisher { get; set; } = new Subject<string>();
  private ISubject<string> InboundMessagePublisher { get; set; } = new Subject<string>();
  private Channel<string> InputChannel { get; }
  private Channel<string> OutputChannel { get; }

  public string? Name { get; set; }

  public bool IsOpen { get; private set; }

  public IObservable<string> OutboundMessages { get; private set; }

  public IObservable<string> InboundMessages { get; private set; }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen)
      throw new Exception($"{Name} not open. Cannot send outbound message");

    OutputChannel.Writer.WaitToWriteAsync(CancellationToken.None).AsTask().ContinueWith(task => {
      if (task.Result)
      {
        OutputChannel.Writer.TryWrite(input);
        OutboundMessagePublisher.OnNext(input);
      }
      else
      {
        throw new InvalidOperationException($"{Name} couldn't write to channel.");
      }
    });

    // var simIo = SimRequestWithResponse<SerialCommandResponse>.GetSimulatedIo(input);
    // OutboundMessagePublisher.OnNext(simIo[0]!);
    // if (simIo[1] == null)
    //   // response not expected
    //   return;
    //
    // var fakeInput = simIo[1];
    // if (fakeInput is null)
    //   return;
    //
    // var random = new Random();
    // Task.Delay(TimeSpan.FromMilliseconds(random.Next(100, 500))).ContinueWith(_ => InputChannel.Writer.TryWrite(fakeInput));
  }

  public void Open(string portName)
  {
    Name = portName;
    IsOpen = Name != null;

    OutboundMessagePublisher = new Subject<string>();
    InboundMessagePublisher = new Subject<string>();

    OutboundMessages = OutboundMessagePublisher.AsObservable();
    InboundMessages = InboundMessagePublisher.AsObservable();
  }

  public void Close(Exception? error = null)
  {
    if (error is null)
    {
      OutboundMessagePublisher.OnCompleted();
      InboundMessagePublisher.OnCompleted();
      // InputChannel.Writer.Complete();
    }
    else
    {
      OutboundMessagePublisher.OnError(error);
      InboundMessagePublisher.OnError(error);
      // InputChannel.Writer.TryWrite(error.Message);// possibly not needed?
    }

    Name = null;
    IsOpen = false;
  }

  public Task ListenForEntryAsync(CancellationToken cancellationToken)
  {
    return Task.Run(
      async () => {
        while (!cancellationToken.IsCancellationRequested)
        {
          if (!IsOpen)
            throw new Exception($"{Name} is not open. Cannot listen for entry");

          try
          {
            var reader = InputChannel.Reader.WaitToReadAsync(cancellationToken).AsTask();
            // To keep it similar to actual hardware logic
            var completedTask = await Task.WhenAny(reader, Task.Delay(1000, cancellationToken));
            if (completedTask != reader)
              continue;

            var result = await InputChannel.Reader.ReadAsync(cancellationToken);
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
