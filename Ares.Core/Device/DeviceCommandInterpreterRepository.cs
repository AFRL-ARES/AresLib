using System.Collections.ObjectModel;
using Ares.Device;

namespace Ares.Core.Device;

internal class DeviceCommandInterpreterRepo : Collection<IDeviceCommandInterpreter<IAresDevice>>, IDeviceCommandInterpreterRepo
{
}
