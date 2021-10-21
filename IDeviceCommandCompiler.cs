using System;
using System.Threading.Tasks;

namespace AresLib
{
  public interface IDeviceCommandCompiler
  {
    Action DeviceAction { init; }
    Task Compile();
  }
}
