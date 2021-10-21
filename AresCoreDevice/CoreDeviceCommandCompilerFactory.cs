using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.AresCoreDevice
{
  internal class CoreDeviceCommandCompilerFactory : DeviceCommandCompilerFactory<CoreDevice>
  {
    protected override Action GetDeviceAction(CommandTemplate commandTemplate)
    {
      if (commandTemplate.Metadata.Name.Equals("WAIT"))
      {
        var duration = TimeSpan.FromSeconds(0);
        var deviceAction = new Action(() => Device.Wait(duration));
        return deviceAction;
      }

      return null;
    }
  }
}
