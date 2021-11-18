using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AresSerial
{
  public class AresSimPort : IAresSerialPort
  {
    public AresSimPort(string name)
    {
      PortName = name;
    }

    public void WriteLine(string input)
    {
      var simIo = SimSerialCommandRequest.GetSimulatedIo(input);
      if (simIo[1] == null)
      {
        // response not expected
        return;
      }

      var rand = new Random();
      var timeout = rand.Next(100, 500); // how long it takes the device to "produce" a response

      Task.Delay(timeout)
          .ContinueWith(async _ => await DeviceChannel.Writer.WriteAsync(simIo[1]));
    }


    public string PortName { get; set; }

    public int ReadTimeout { get; set; } = 1000;

    public bool IsOpen { get; private set; }

    public virtual void Open()
    {
      IsOpen = true;
    }

    public string ReadLine()
    {
      var cancellationTokenSource = new CancellationTokenSource();
      var valueTask = DeviceChannel.Reader.WaitToReadAsync(cancellationTokenSource.Token).AsTask();
      var timeoutTask = Task.Delay(ReadTimeout);
      var completed = Task.WhenAny(valueTask, timeoutTask).Result;
      if (completed == timeoutTask)
      {
        cancellationTokenSource.Cancel();
        throw new TimeoutException("SimPort ReadLine timed out");
      }
      var resultRead = DeviceChannel.Reader.TryRead(out var result);
      if (!resultRead)
        throw new Exception("Sim DeviceChannel is closed");
      return result;
    }

    private Channel<string> DeviceChannel { get; } = Channel.CreateBounded<string>(1);
  }
}
