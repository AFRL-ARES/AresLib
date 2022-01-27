using System;
using System.Linq;
using System.Threading.Tasks;
using Ares.Messaging;

namespace Ares.Device
{
  public abstract class DeviceCommandInterpreter<TQualifiedDevice, DeviceCommandEnum>
    : IDeviceCommandInterpreter<TQualifiedDevice>
    where TQualifiedDevice : IAresDevice
    where DeviceCommandEnum : struct, Enum
  {
    protected DeviceCommandInterpreter(TQualifiedDevice device)
    {
      Device = device;
    }

    public CommandMetadata[] CommandsToIndexedMetadatas()
    {
      var commandMetadatas = CommandsToMetadatas();
      foreach (var commandMetadata in commandMetadatas)
        OrderCommandMetadata(commandMetadata);

      return commandMetadatas;
    }

    public Task TemplateToDeviceCommand(CommandTemplate commandTemplate)
    {
      Action qualifiedDeviceAction = () => RouteDeviceAction(commandTemplate);
      return new Task(qualifiedDeviceAction);
    }

    public TQualifiedDevice Device { get; }

    // NOTE: The intent of this command is to prevent protobuf message exposure to extensions. 
    // We want this abstract class to handle as much conversion/routing of protobuf/db to
    // lib representations as possible, making it easier/obvious for extensions to "know what to do".
    // couldn't think of something better to say, but its a comment that will get deleted anyway.
    protected abstract void ParseAndPerformDeviceAction(DeviceCommandEnum deviceCommandEnum, Parameter[] parameters);

    private void RouteDeviceAction(CommandTemplate commandTemplate)
    {
      var deviceCommandEnum = Enum.Parse<DeviceCommandEnum>(commandTemplate.Metadata.Name);
      var arguments = commandTemplate.Arguments.OrderBy(argument => argument.Index).ToArray();
      ParseAndPerformDeviceAction(deviceCommandEnum, arguments);
    }

    protected abstract CommandMetadata[] CommandsToMetadatas();

    private void OrderCommandMetadata(CommandMetadata commandMetadata)
    {
      var parameterMetadatasAscending = commandMetadata.ParameterMetadatas.ToArray();

      if (parameterMetadatasAscending.Length > 1)
      {
        if (parameterMetadatasAscending.All(parameterMetadata => parameterMetadata.Index == default))
        {
          parameterMetadatasAscending = parameterMetadatasAscending.Select
            (
              (parameter, index) => {
                parameter.Index = index;
                return parameter;
              }
            )
            .ToArray();
        }
        else
        {
          // Improperly ordered
          var distinctParameterIndexes =
            parameterMetadatasAscending
              .Select(parameter => parameter.Index)
              .Distinct()
              .ToArray();

          if (distinctParameterIndexes.Length != parameterMetadatasAscending.Length)
            throw new Exception($"{GetType().Name} error parsing {commandMetadata.Name} parameters, parameter Indexes are not distinct");
        }
      }

      parameterMetadatasAscending = parameterMetadatasAscending.OrderBy(parameterMetadata => parameterMetadata.Index)
        .ToArray();

      commandMetadata.ParameterMetadatas.Clear();
      commandMetadata.ParameterMetadatas.AddRange(parameterMetadatasAscending);
    }
  }

}
