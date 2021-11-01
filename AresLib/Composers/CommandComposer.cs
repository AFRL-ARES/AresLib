using AresLib.Executors;
using Google.Protobuf;
using System.Collections.ObjectModel;
using AresDevicePluginBase;

namespace AresLib.Composers
{
  internal abstract class CommandComposer<TDbTemplate, TExecutor> : ICommandComposer<TDbTemplate, TExecutor>
    where TDbTemplate : IMessage
    where TExecutor : IBaseExecutor
  {
    protected CommandComposer(TDbTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> availableDeviceCommandInterpreters)
    {
      Template = template;
      AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
    }
    public abstract TExecutor Compose();

    public TDbTemplate Template { get; }

    public ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> AvailableDeviceCommandInterpreters { get; protected set; }
  }
}
