using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib.AresCoreDevice
{
  internal class CoreDeviceDomainTranslator : DeviceDomainTranslator<CoreDeviceCommand>
  {
    protected override AresCommand GenerateAresCommand(DeviceCommand deviceCommand)
    {

    }

    protected override CoreDeviceCommand GenerateDeviceSpecifiedCommand(AresCommand deviceCommand)
    {
      throw new NotImplementedException();
    }

    internal CoreDeviceCommand GenerateWaitCommand(AresCommand aresCommand)
    {
      // parse out delay from command
      //aresCommand.Arguments.First().Value
      var delay = TimeSpan.FromSeconds(1); 
      var command = new CoreDeviceCommand {  };
    }
  }
}
