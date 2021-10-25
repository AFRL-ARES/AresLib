using Ares.Core;
using DynamicData;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class DeviceCommandCompilerFactory<TQualifiedDevice>
    : IDeviceCommandCompilerFactory<TQualifiedDevice>
    where TQualifiedDevice : AresDevice
  {
    protected DeviceCommandCompilerFactory()
    {
      AvailableCommandMetadatasSource
        .Connect()
        .Bind(out var readonlyAvailableCommandMetadatas)
        .Subscribe();

      AvailableCommandMetadatas = readonlyAvailableCommandMetadatas;
    }

    public IDeviceCommandCompiler Create(CommandTemplate commandTemplate)
    {
      var qualifiedDeviceAction = GetDeviceAction(commandTemplate);
      return new DeviceCommandCompiler { DeviceAction = qualifiedDeviceAction };
    }


    protected abstract Func<Task> GetDeviceAction(CommandTemplate commandTemplate);

    public TQualifiedDevice Device { get; init; }

    public ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; }

    protected ISourceCache<CommandMetadata, string> AvailableCommandMetadatasSource { get; }
      = new SourceCache<CommandMetadata, string>(commandMetadata => commandMetadata.Name);
  }

}
