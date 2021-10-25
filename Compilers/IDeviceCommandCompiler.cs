using System;
using System.Threading.Tasks;

namespace AresLib.Compilers
{
  public interface IDeviceCommandCompiler
  {
    Func<Task> DeviceAction { init; }
    Func<Task> Compile();
  }
}
