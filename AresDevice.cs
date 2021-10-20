using System;
using System.Collections.ObjectModel;

namespace AresLib
{
  public abstract class AresDevice<T> : IAresDevice where T : DeviceCommand, new()
  {
    public Guid Id { get; } = Guid.NewGuid();

    public void ExecuteAbstractCommand(DeviceCommand command)
    {
      ExecuteGenericCommand((T) command);
    }

    protected abstract void ExecuteGenericCommand(T deviceSpecificCommand);

    public ReadOnlyObservableCollection<CommandMetadata> AvailableCommands { get; }
  }
}
