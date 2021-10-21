using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace AresLib
{
  public class DeviceCommandCompilerRepoBridge
  {
    public ISourceCache<IDeviceCommandCompiler<AresDevice>, string> Repo { get; } 
      = new SourceCache<IDeviceCommandCompiler<AresDevice>, string>(compiler => compiler.Device.Name);
  }
}
