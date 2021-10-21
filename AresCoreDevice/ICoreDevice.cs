using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib.AresCoreDevice
{
  internal interface ICoreDevice : IAresDevice
  {
    void Wait(TimeSpan duration);
  }
}
