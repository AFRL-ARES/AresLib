using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class DeviceDomainTranslator<T> : IDeviceDomainTranslator where T : DeviceCommand, new()
  {
    public AresCommand GenerateAresCommand(DeviceCommand deviceCommand)
    {
      return GenerateAresCommand((T) deviceCommand);
    }

    public DeviceCommand GenerateDeviceCommand(AresCommand aresCommand)
    {
      return GenerateDeviceSpecifiedCommand(aresCommand);
    }

    protected abstract AresCommand GenerateAresCommand(T deviceCommand);
    protected abstract T GenerateDeviceSpecifiedCommand(AresCommand deviceCommand);
  }
}
