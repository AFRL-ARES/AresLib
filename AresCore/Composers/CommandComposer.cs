using System.Collections.ObjectModel;
using Ares.Core.Executors;
using Ares.Device;
using Google.Protobuf;

namespace Ares.Core.Composers;

internal abstract class CommandComposer<TDbTemplate, TExecutor> : ICommandComposer<TDbTemplate, TExecutor>
  where TDbTemplate : IMessage
  where TExecutor : IBaseExecutor
{
  protected CommandComposer(TDbTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters)
  {
    Template = template;
    AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
  }

  public ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> AvailableDeviceCommandInterpreters { get; protected set; }
  public abstract TExecutor Compose();

  public TDbTemplate Template { get; }
}
