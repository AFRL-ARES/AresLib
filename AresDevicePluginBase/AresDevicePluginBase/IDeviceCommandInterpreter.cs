using System;
using System.Threading;
using System.Threading.Tasks;
using Ares.Messaging;

namespace Ares.Device;

public interface IDeviceCommandInterpreter<out TQualifiedDevice>
  where TQualifiedDevice : IAresDevice
{
  TQualifiedDevice Device { get; }
  Func<CancellationToken, Task> TemplateToDeviceCommand(CommandTemplate commandTemplate);
  CommandMetadata[] CommandsToIndexedMetadatas();
}
