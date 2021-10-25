using System;
using System.Threading.Tasks;

namespace AresLib.AresCoreDevice
{
  internal interface ICoreDevice : IAresDevice
  {
    Task Wait(TimeSpan duration);
  }
}
