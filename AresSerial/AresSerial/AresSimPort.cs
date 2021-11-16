using System;
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

      Task.Delay(200)
          .ContinueWith(_ => DeviceOutput.OnNext(simIo[1]));
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
      var valueTask = DeviceOutput.FirstAsync().ToTask();
      var timeoutTask = Task.Delay(ReadTimeout);
      var completed = Task.WhenAny(valueTask, timeoutTask).Result;
      if (completed == timeoutTask)
        throw new TimeoutException("SimPort ReadLine timed out");
      return valueTask.Result;
    }

    protected Subject<string> DeviceOutput { get; } = new Subject<string>();
  }
}
