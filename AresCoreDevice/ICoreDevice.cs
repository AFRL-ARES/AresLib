using System;

namespace AresLib.AresCoreDevice
{
  internal interface ICoreDevice : IAresDevice
  {
    void Wait(TimeSpan duration);
  }
}
