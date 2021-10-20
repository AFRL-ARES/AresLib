using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AresLib
{
  public interface IAresDevice
  {
    Guid Id { get; }
    string Name { get; }
    void ExecuteAbstractCommand(DeviceCommand command);
    ReadOnlyObservableCollection<CommandMetadata> AvailableCommands { get; }
  }
}
