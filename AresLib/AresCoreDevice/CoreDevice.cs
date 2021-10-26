using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AresLib.AresCoreDevice
{
  internal class CoreDevice : AresDevice, ICoreDevice
  {
    public CoreDevice()
    {
      Name = "Core";
    }
    public async Task Wait(TimeSpan duration)
    {
      await Task.Delay(duration);
      Console.WriteLine($"Waited for {duration.TotalSeconds} seconds.");

    }
  }
}
