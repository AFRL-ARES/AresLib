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
    var thread = new Thread(
      () => {
        Thread.CurrentThread.Name ??= $"{Name} Sim Device Receiver";
        Thread.CurrentThread.IsBackground = true;
        while (!token.IsCancellationRequested)
        {
          var reader = InputChannel.Reader.WaitToReadAsync(token).AsTask();
          var completedTask = Task.WhenAny(reader, Task.Delay(1000, token));
          completedTask.Wait(token);
          if (completedTask.Result != reader)
            continue;

          var readSuccess = InputChannel.Reader.TryRead(out var result);
          if (readSuccess && result is not null)
            Receive(result);
        }
      }
    );

    thread.Start();
  }
}
