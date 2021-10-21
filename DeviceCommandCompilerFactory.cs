using Ares.Core;
using System;

namespace AresLib
{
  internal abstract class DeviceCommandCompilerFactory<QualifiedDevice> where QualifiedDevice : AresDevice
  {
    public QualifiedDevice Device { get; init; }

    public IDeviceCommandCompiler Create(CommandTemplate commandTemplate)
    {
      var qualifiedDeviceAction = GetDeviceAction(commandTemplate);
      return new DeviceCommandCompiler { DeviceAction = qualifiedDeviceAction };
    }

    protected abstract Action GetDeviceAction(CommandTemplate commandTemplate);
  }

}
