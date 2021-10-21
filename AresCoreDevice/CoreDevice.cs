using System;
using System.Threading;

namespace AresLib.AresCoreDevice
{
  internal class CoreDevice : AresDevice, ICoreDevice
  {
    public void Wait(TimeSpan duration)
    {
      Thread.Sleep(duration);
    }
  }
}
