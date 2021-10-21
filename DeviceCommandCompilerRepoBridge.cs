using DynamicData;

namespace AresLib
{
  public class DeviceCommandCompilerRepoBridge
  {
    public ISourceCache<IDeviceCommandCompilerFactory<AresDevice>, string> Repo { get; }
      = new SourceCache<IDeviceCommandCompilerFactory<AresDevice>, string>(compiler => compiler.Device.Name);
  }
}
