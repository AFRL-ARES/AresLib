using System;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device;

public interface IAresDevice
{
  string Name { get; }
  DeviceStatus Status { get; }
  bool Activate();
}
