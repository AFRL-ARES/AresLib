using AresLib.AresCoreDevice;
using DynamicData;

namespace AresLib.Compilers
{
  // TODO: Consider moving this style logic into Lab/LabManager stuff (delete this class, seems to be stylized for injection-style stuff)
  public class DeviceCommandCompilerFactoryRepoBridge
  {
    public ISourceCache<IDeviceCommandCompilerFactory<AresDevice>, string> Repo { get; }
      = new SourceCache<IDeviceCommandCompilerFactory<AresDevice>, string>(compiler => compiler.Device.Name);
  }
}
