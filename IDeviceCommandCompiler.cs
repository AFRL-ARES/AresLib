using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  public interface IDeviceCommandCompiler
  {
    Action DeviceAction { init; }
    Task Compile();
  }
}
