using System.Threading.Tasks;

namespace AresDevicePluginBase
{
  public abstract class AresDevice : IAresDevice
  {
    protected AresDevice(string name)
    {
      Name = name;
    }
    public string Name { get; }
    public abstract Task<bool> Activate();
  }
}
