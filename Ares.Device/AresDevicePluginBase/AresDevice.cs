using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Ares.Messaging.Device;
using Grpc.Core;

namespace Ares.Device;

public abstract class AresDevice : IAresDevice
{
  protected AresDevice(string name)
  {
    Name = name;
    Status = new DeviceStatus
      { DeviceState = DeviceState.Inactive, Message = $"{Name} constructed. Activation has not been called yet." };
  }

  public string Name { get; }
  public DeviceStatus Status { get; protected set; }
  public abstract bool Activate();
}
