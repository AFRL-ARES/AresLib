using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ares.Device.Serial.Simulation;

public abstract class SimAresDevice
{
  private readonly CancellationTokenSource _source = new();

  public SimAresDevice(string name)
  {
    InputChannel = Channel.CreateBounded<string>(1);
    OutputChannel = Channel.CreateBounded<string>(1);
    Name = name;
    SetupReceive();
  }

  public string Name { get; }

  internal Channel<string> InputChannel { get; }
  internal Channel<string> OutputChannel { get; }

  public abstract void Receive(string message);

  protected void Send(string message)
  {
    OutputChannel.Writer.WaitToWriteAsync(CancellationToken.None).AsTask().ContinueWith(task => {
      if (task.Result)
        OutputChannel.Writer.TryWrite(message);
      else
        throw new InvalidOperationException($"{Name} couldn't write to channel.");
    });
  }

  private void SetupReceive()
  {
    var token = _source.Token;
    Task.Run(
      async () => {
        Thread.CurrentThread.Name ??= $"{Name} Sim Device Receiver";
        while (!token.IsCancellationRequested)
        {
          var reader = InputChannel.Reader.WaitToReadAsync(token).AsTask();
          var completedTask = await Task.WhenAny(reader, Task.Delay(1000, token));
          if (completedTask != reader)
            continue;

          var result = await InputChannel.Reader.ReadAsync(token);
          Receive(result);
        }
      },
      token
    );
  }
}
