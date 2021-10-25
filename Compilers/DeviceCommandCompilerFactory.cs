using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib.Compilers
{
  internal abstract class DeviceCommandCompilerFactory<TQualifiedDevice, DeviceCommandEnum>
    : IDeviceCommandCompilerFactory<TQualifiedDevice>
    where TQualifiedDevice : AresDevice
    where DeviceCommandEnum : struct, Enum
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
      Action qualifiedDeviceAction = () => RouteDeviceAction(commandTemplate);
      return new DeviceCommandCompiler { DeviceAction = qualifiedDeviceAction };
    }


    protected void RouteDeviceAction(CommandTemplate commandTemplate)
    {
      var deviceCommandEnum = Enum.Parse<DeviceCommandEnum>(commandTemplate.Metadata.Name);
      var arguments = commandTemplate.Arguments.ToArray();
      ParseAndPerformDeviceAction(deviceCommandEnum, arguments);
    }

    // NOTE: The intent of this command is to prevent protobuf message exposure to extensions. 
    // We want this abstract class to handle as much conversion/routing of protobuf/db to
    // lib representations as possible, making it easier/obvious for extensions to "know what to do".
    // couldn't think of something better to say, but its a comment that will get deleted anyway.
    protected abstract void ParseAndPerformDeviceAction(DeviceCommandEnum deviceCommandEnum, CommandParameter[] commandParameters);

    public TQualifiedDevice Device { get; init; }

    public ReadOnlyObservableCollection<CommandMetadata> AvailableCommandMetadatas { get; }

    protected ISourceCache<CommandMetadata, string> AvailableCommandMetadatasSource { get; }
      = new SourceCache<CommandMetadata, string>(commandMetadata => commandMetadata.Name);
  }

}
