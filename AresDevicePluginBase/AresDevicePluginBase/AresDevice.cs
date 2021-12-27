using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Ares.Core.Messages.Device;
namespace AresDevicePluginBase
{
  public abstract class AresDevice : IAresDevice
  {
    protected readonly ISubject<DeviceStatus> StatusPublisher
      = new BehaviorSubject<DeviceStatus>(new DeviceStatus { DeviceState = DeviceState.Inactive });

    protected AresDevice(string name)
    {
      Name = name;
      Status = StatusPublisher.AsObservable();
    }
    public string Name { get; }
    public IObservable<DeviceStatus> Status { get; }
    public abstract Task<bool> Activate();
  }
}
