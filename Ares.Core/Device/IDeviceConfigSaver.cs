using Ares.Messaging.Device;

namespace Ares.Core.Device;

public interface IDeviceConfigSaver
{
  Task AddConfig(DeviceConfig config);
  Task RemoveConfig(Guid configId);
  Task UpdateConfig(DeviceConfig config);
}
