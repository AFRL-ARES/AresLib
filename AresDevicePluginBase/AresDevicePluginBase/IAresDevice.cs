using System;
using System.Threading.Tasks;
using Ares.Core.Messages.Device;
namespace AresDevicePluginBase
{
  public interface IAresDevice
  {
    string Name { get; }
    IObservable<DeviceStatus> Status { get; }
    Task<bool> Activate();
  }
}
