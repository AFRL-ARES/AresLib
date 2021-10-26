using System;
using System.Threading.Tasks;

namespace AresLib.Compilers
{
  public interface IDeviceCommandCompiler
  {
    Action DeviceAction { init; }
    Task Compile();
  }
}
