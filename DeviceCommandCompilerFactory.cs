using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal abstract class DeviceCommandCompilerFactory<QualifiedDevice> where QualifiedDevice : AresDevice
  {
    public QualifiedDevice Device { get; init; }

    public IDeviceCommandCompiler Create(CommandTemplate commandTemplate)
    {
      var qualifiedDeviceAction = GetDeviceAction(commandTemplate);
      return new DeviceCommandCompiler {DeviceAction = qualifiedDeviceAction};
    }

    protected abstract Action GetDeviceAction(CommandTemplate commandTemplate);
  }

}
