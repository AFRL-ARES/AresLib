using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib.Compilers
{
  internal abstract class DeviceCommandInterpreter<TQualifiedDevice, DeviceCommandEnum>
    : IDeviceCommandInterpreter<TQualifiedDevice>
    where TQualifiedDevice : AresDevice
    where DeviceCommandEnum : struct, Enum
  {

    // NOTE: The intent of this command is to prevent protobuf message exposure to extensions. 
    // We want this abstract class to handle as much conversion/routing of protobuf/db to
    // lib representations as possible, making it easier/obvious for extensions to "know what to do".
    // couldn't think of something better to say, but its a comment that will get deleted anyway.
    protected abstract void ParseAndPerformDeviceAction(DeviceCommandEnum deviceCommandEnum, CommandParameter[] commandParameters);
    
    private void RouteDeviceAction(CommandTemplate commandTemplate)
    {
      var deviceCommandEnum = Enum.Parse<DeviceCommandEnum>(commandTemplate.Metadata.Name);
      var arguments = commandTemplate.Arguments.ToArray();
      ParseAndPerformDeviceAction(deviceCommandEnum, arguments);
    }

    public abstract CommandMetadata[] CommandsToMetadatas();


    public Task TemplateToDeviceCommand(CommandTemplate commandTemplate)
    {
      Action qualifiedDeviceAction = () => RouteDeviceAction(commandTemplate);
      return new Task(qualifiedDeviceAction);
    }

    public TQualifiedDevice Device { get; init; }
  }

}
