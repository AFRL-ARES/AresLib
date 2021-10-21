using Ares.Core;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DynamicData;

namespace AresLib
{
  internal abstract class DeviceCommandCompilerFactory<QualifiedDevice, DeviceCommandEnum> 
    : IDeviceCommandCompilerFactory<QualifiedDevice, DeviceCommandEnum>
    where QualifiedDevice : AresDevice
    where DeviceCommandEnum : struct, Enum
  {
    protected DeviceCommandCompilerFactory()
    {
      AvailableCommandMetadatasSource
        .Connect()
        .Bind(out var readonlyAvailableCommandMetadatas)
        .Subscribe();

      AvailableCommandMetadatas = readonlyAvailableCommandMetadatas;

      var enumeratedCommandEnums = Enum.GetValues<DeviceCommandEnum>();

    }

    public void RegisterCommandMetadatas()
    {
      var deviceCommandEnumValues = Enum.GetValues<DeviceCommandEnum>();
      var commandMetadatas = deviceCommandEnumValues.Select(GenerateCommandMetadata).ToArray();
      AvailableCommandMetadatasSource.AddOrUpdate(commandMetadatas);
    }


    public abstract CommandMetadata GenerateCommandMetadata(DeviceCommandEnum deviceCommandEnum);

    public IDeviceCommandCompiler Create(CommandTemplate commandTemplate)
    {
      var qualifiedDeviceAction = GetDeviceAction(commandTemplate);
      return new DeviceCommandCompiler { DeviceAction = qualifiedDeviceAction };
    }


    protected abstract Action GetDeviceAction(CommandTemplate commandTemplate);

    public QualifiedDevice Device { get; init; }

    public ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; }

    protected ISourceCache<CommandMetadata, string> AvailableCommandMetadatasSource { get; }
      = new SourceCache<CommandMetadata, string>(commandMetadata => commandMetadata.Name);
  }

}
