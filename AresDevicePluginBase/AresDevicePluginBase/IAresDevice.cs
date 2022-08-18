using System;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device;

public interface IAresDevice
{
  string Name { get; }
  IObservable<DeviceStatus> Status { get; }
  Task<bool> Activate();
}