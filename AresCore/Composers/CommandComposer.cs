using System.Collections.ObjectModel;
using Ares.Device;
using Google.Protobuf;

namespace Ares.Core.Composers;

internal abstract class CommandComposer<TDbTemplate, TExecutor> : ICommandComposer<TDbTemplate, TExecutor>
  where TDbTemplate : IMessage
{
  protected CommandComposer(TDbTemplate template, IEnumerable<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters)
  {
    Template = template;
    AvailableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
  }

  public IEnumerable<IDeviceCommandInterpreter<IAresDevice>> AvailableDeviceCommandInterpreters { get; protected set; }
  public abstract TExecutor Compose();

  public TDbTemplate Template { get; }
}
