using System;
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
    public AresSimPort(string name)
    {
      PortName = name;
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
      var valueTask = ValProvider.FirstAsync().ToTask();
      var timeoutTask = Task.Delay(ReadTimeout);
      var completed = Task.WhenAny(valueTask, timeoutTask).Result;
      if (completed == timeoutTask)
        throw new TimeoutException("SimPort ReadLine timed out");
      return valueTask.Result;
    }

    public virtual void WriteLine(string input)
    {
      Thread.Sleep(TimeSpan.FromSeconds(1));
      ValProvider.OnNext($"SIM {input}");
    }

    protected Subject<string> ValProvider { get; } = new Subject<string>();
  }
}
