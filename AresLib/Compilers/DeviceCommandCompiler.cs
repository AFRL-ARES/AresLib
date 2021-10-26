using System;
using System.Threading.Tasks;

namespace AresLib.Compilers
{
  internal class DeviceCommandCompiler : IDeviceCommandCompiler
  {

    public Task Compile()
    {
      return new Task(DeviceAction);
    }

    public Action DeviceAction { get; init; }
  }
}
