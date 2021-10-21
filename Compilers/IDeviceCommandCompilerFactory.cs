using System;
using System.Collections.ObjectModel;
using Ares.Core;

namespace AresLib
{
  public interface IDeviceCommandCompilerFactory<QualifiedDevice, DeviceCommandEnum> 
    where QualifiedDevice : IAresDevice
    where DeviceCommandEnum : Enum
  {
    IDeviceCommandCompiler Create(CommandTemplate commandTemplate);
    void RegisterCommandMetadatas();
    QualifiedDevice Device { get; init; }
    ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; init; }
  }
}
