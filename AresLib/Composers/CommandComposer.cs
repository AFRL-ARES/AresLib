using AresLib.Device;
using AresLib.Executors;
using Google.Protobuf;
using System.Collections.ObjectModel;

namespace AresLib.Composers
{
  internal abstract class CommandComposer<DbTemplate, CoreExecutable> : ICommandComposer<DbTemplate, CoreExecutable>
    where DbTemplate : IMessage
    where CoreExecutable : IBaseExecutor
  {
    protected CommandComposer(DbTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> availableDeviceCommandInterpreters)
    {
      Template = template;
      AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
    }
    public abstract CoreExecutable Compose();

    public DbTemplate Template { get; }

    public ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> AvailableDeviceCommandInterpreters { get; protected set; }
  }
}
