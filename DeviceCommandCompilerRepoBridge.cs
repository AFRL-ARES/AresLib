using DynamicData;

namespace AresLib
{
  public class DeviceCommandCompilerRepoBridge
  {
    public ISourceCache<IDeviceCommandCompiler, string> Repo { get; }
      = new SourceCache<IDeviceCommandCompiler, string>(compiler => compiler.Device.Name);
  }
}
