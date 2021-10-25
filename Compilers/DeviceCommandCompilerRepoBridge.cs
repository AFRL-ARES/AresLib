using System;
using System.Collections.Generic;
using AresLib.AresCoreDevice;
using DynamicData;

namespace AresLib
{
  public class DeviceCommandCompilerRepoBridge
  {
    public DeviceCommandCompilerRepoBridge()
    {
      var coreFac = new CoreDeviceCommandCompilerFactory();
      Repo.AddOrUpdate(coreFac);
    }
    public ISourceCache<IDeviceCommandCompilerFactory<AresDevice>, string> Repo { get; }
      = new SourceCache<IDeviceCommandCompilerFactory<AresDevice>, string>(compiler => compiler.Device.Name);
  }
}
