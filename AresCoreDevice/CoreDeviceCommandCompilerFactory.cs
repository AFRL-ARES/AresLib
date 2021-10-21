using Ares.Core;
using System;

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
