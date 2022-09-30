using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ares.Device.Serial.Simulation;

internal class AresSimPort : IAresSerialPort
{
  public AresSimPort(Channel<string> inputChannel, Channel<string> outputChannel)
  {
    InputChannel = inputChannel;
    OutputChannel = outputChannel;
  }

  private Channel<string> InputChannel { get; }
  private Channel<string> OutputChannel { get; }

  public string? Name { get; set; }

  public bool IsOpen { get; private set; }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen)
      throw new Exception($"{Name} not open. Cannot send outbound message");

    OutputChannel.Writer.WaitToWriteAsync(CancellationToken.None).AsTask().ContinueWith(task => {
      if (task.Result)
        OutputChannel.Writer.TryWrite(input);
      else
        throw new InvalidOperationException($"{Name} couldn't write to channel.");
    });
  }

  public void Open(string portName)
  {
    Name = portName;
    IsOpen = Name != null;
  }

  public void Close(Exception? error = null)
  {
    Name = null;
    IsOpen = false;
  }

  public Task<string> ListenForEntryAsync(CancellationToken cancellationToken, TimeSpan timeout)
  {
    return Task.Run(
      async () => {
        if (!IsOpen)
          throw new Exception($"{Name} is not open. Cannot listen for entry");

        var reader = InputChannel.Reader.WaitToReadAsync(cancellationToken).AsTask();
        // To keep it similar to actual hardware logic
        var completedTask = await Task.WhenAny(reader, Task.Delay(10000, cancellationToken));
        if (completedTask != reader)
          throw new TimeoutException();

        var result = await InputChannel.Reader.ReadAsync(cancellationToken);
        return result;
      },
      cancellationToken
    );
  }
}
