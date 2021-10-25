using Ares.Core;
using System;
using System.Collections.ObjectModel;

namespace AresLib
{
  public interface IDeviceCommandCompilerFactory<out TQualifiedDevice>
    where TQualifiedDevice : IAresDevice
  {
    IDeviceCommandCompiler Create(CommandTemplate commandTemplate);
    TQualifiedDevice Device { get; }
    ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; }
  }
}
