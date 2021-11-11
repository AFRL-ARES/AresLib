using System;
using System.Threading.Tasks;

namespace AresDevicePluginBase
{
  public interface IAresDevice
  {
    string Name { get; }
    Task<bool> Activate();
  }
}
