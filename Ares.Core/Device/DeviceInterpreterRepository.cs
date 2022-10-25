using System.Collections.ObjectModel;
using Ares.Device;

namespace Ares.Core.Device;

internal class DeviceInterpreterRepository : Collection<IDeviceCommandInterpreter<IAresDevice>>
{
}
