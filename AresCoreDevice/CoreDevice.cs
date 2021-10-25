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
    public Task Wait(TimeSpan duration)
    {
      Console.WriteLine($"Waiting for {duration.TotalSeconds} seconds.");
      return Task.Delay(duration);
    }
  }
}
