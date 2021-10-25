using AresLib.AresCoreDevice;
using DynamicData;

namespace AresLib.Compilers
{
  public class DeviceCommandCompilerFactoryRepoBridge
  {
    public ISourceCache<IDeviceCommandCompilerFactory<AresDevice>, string> Repo { get; }
      = new SourceCache<IDeviceCommandCompilerFactory<AresDevice>, string>(compiler => compiler.Device.Name);
  }
}
