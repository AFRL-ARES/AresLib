using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib.AresCoreDevice
{
  internal interface ICoreDevice : IAresDevice<CoreDeviceCommand>
  {
    void Wait(TimeSpan duration);
  }
}
