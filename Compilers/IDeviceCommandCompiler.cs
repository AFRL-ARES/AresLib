using System;
using System.Threading.Tasks;

namespace AresLib
{
  public interface IDeviceCommandCompiler
  {
    Func<Task> DeviceAction { init; }
    Func<Task> Compile();
  }
}
