using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.Device
{
  public interface IDeviceCommandInterpreter<out TQualifiedDevice>
    where TQualifiedDevice : AresDevice
  {
    Task TemplateToDeviceCommand(CommandTemplate commandTemplate);
    CommandMetadata[] CommandsToMetadatas();
    TQualifiedDevice Device { get; }
  }
}
