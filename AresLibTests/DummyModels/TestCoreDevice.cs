using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib;
using AresLib.Device;

namespace AresLibTests.DummyModels
{
  public class TestCoreDevice : AresDevice, ITestCoreDevice
  {
    public TestCoreDevice(int adress) : base($"TestCoreDevice_{adress}")
    {
      Address = adress;
    }
    public async Task Wait(TimeSpan duration)
    {
      var start = DateTime.Now;
      await Task.Delay(duration);
      var finish = DateTime.Now;
      var executionTime = finish - start;
      Console.Write($"Delayed {executionTime.TotalSeconds}s");
    }

    public void Derp()
    {
      Console.WriteLine("Executed Derp");
    }

    public int Test()
    {
      Console.Write("Executed Test (command)");
      return 7;
    }

    public int Address { get; }
  }
}
