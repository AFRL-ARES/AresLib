using System.Collections.ObjectModel;
using Ares.Core;

namespace AresLib.Compilers
{
  public interface IDeviceCommandCompilerFactory<out TQualifiedDevice>
    where TQualifiedDevice : IAresDevice
  {
    IDeviceCommandCompiler Create(CommandTemplate commandTemplate);
    TQualifiedDevice Device { get; }
    ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; }
  }
}
