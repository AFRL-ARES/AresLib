using System.Threading.Tasks;
using Ares.Core;

namespace AresDevicePluginBase
{
  public interface IDeviceCommandInterpreter<out TQualifiedDevice>
    where TQualifiedDevice : AresDevice
  {
    Task TemplateToDeviceCommand(CommandTemplate commandTemplate);
    CommandMetadata[] CommandsToIndexedMetadatas();
    TQualifiedDevice Device { get; }
  }
}
