using System.Threading.Tasks;
using Ares.Messaging;

namespace Ares.Device
{
  public interface IDeviceCommandInterpreter<out TQualifiedDevice>
    where TQualifiedDevice : IAresDevice
  {
    TQualifiedDevice Device { get; }
    Task TemplateToDeviceCommand(CommandTemplate commandTemplate);
    CommandMetadata[] CommandsToIndexedMetadatas();
  }
}
