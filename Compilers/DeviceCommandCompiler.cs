using System;
using System.Threading.Tasks;

namespace AresLib.Compilers
{
  internal class DeviceCommandCompiler : IDeviceCommandCompiler
  {

    public Func<Task> Compile()
    {
      return DeviceAction;
    }

    public Func<Task> DeviceAction { get; init; }
  }
}
