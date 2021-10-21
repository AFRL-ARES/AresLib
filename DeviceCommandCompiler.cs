using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal class DeviceCommandCompiler : IDeviceCommandCompiler
  {

    public Task Compile()
    {
      return new Task(DeviceAction);
    }

    public Action DeviceAction { get; set; }
  }
}
