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
      await Task.Delay(duration);
      Console.WriteLine($"Executed Wait");
    }

    public void Derp()
    {
      Console.WriteLine("Executed Derp");
    }

    public int Test()
    {
      Console.WriteLine("Executed Test (command)");
      return 7;
    }

    public int Address { get; }
  }
}
