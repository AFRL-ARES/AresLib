#nullable enable
using Ares.Core;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitsNet.Units;

namespace AresLib.AresCoreDevice
{
  internal class CoreDeviceCommandCompilerFactory : DeviceCommandCompilerFactory<CoreDevice>
  {
    public CoreDeviceCommandCompilerFactory()
    {
      Device = new CoreDevice();
    }

    public void RegisterCommands()
    {
      var enums =
        Enum.GetValues<CoreDeviceCommandType>()
          .ToArray();
    }

    private CommandParameterMetadata GenerateParameterMetadata(string parameterName, Enum unit)
    {
      var parameterMetadata =
        new CommandParameterMetadata
        {
          Name = $"{parameterName}",
          Unit = $"{unit}"
        };

      return parameterMetadata;
    }

    private void AddOrUpdateCommandMetadata(CoreDeviceCommandType commandType, string description)
    {
      var parameters = new List<CommandParameterMetadata>();

      switch (commandType)
      {

        case CoreDeviceCommandType.Delay:
        case CoreDeviceCommandType.Wait:
          parameters.Add(GenerateParameterMetadata("duration", DurationUnit.Second));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
      }
      var commandMetadata =
        new CommandMetadata
        {
          DeviceName = $"{Device.Name}",
          Name = $"{commandType}",
          Description = description,
          ParameterMetadatas =
          {
            parameters
          }
        };

      AvailableCommandMetadatasSource.AddOrUpdate(commandMetadata);
    }

    /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="CoreDeviceCommandType"/> unsupported by command metadata generator.</exception>
    public CommandMetadata GenerateCommandMetadata(CoreDeviceCommandType commandType)
    {
      var parameters = new List<CommandParameterMetadata>();
      string description;

      switch (commandType)
      {

        case CoreDeviceCommandType.Delay:
        case CoreDeviceCommandType.Wait:
          parameters.Add(GenerateParameterMetadata("duration", DurationUnit.Second));
          description = "Waits a given amount of time before continuing.\n";
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
      }
      var commandMetadata =
        new CommandMetadata
        {
          DeviceName = $"{Device.Name}",
          Name = $"{commandType}",
          Description = description,
          ParameterMetadatas =
          {
            parameters
          }
        };

      return commandMetadata;
    }

    protected override Func<Task>? GetDeviceAction(CommandTemplate commandTemplate)
    {
      if (commandTemplate.Metadata.Name.Equals(CoreDeviceCommandType.Wait.ToString(), StringComparison.OrdinalIgnoreCase) || commandTemplate.Metadata.Name.Equals(CoreDeviceCommandType.Delay.ToString(), StringComparison.OrdinalIgnoreCase))
      {
        return GenerateWaitAction(commandTemplate);
      }

      return null;
    }

    private Func<Task> GenerateWaitAction(CommandTemplate commandTemplate)
    {
      var durationVal = commandTemplate.Arguments.FirstOrDefault();
      if (durationVal is null)
        throw new ArgumentException($"Wait command expected one argument, but no arguments were passed in.");
      var duration = TimeSpan.FromSeconds(durationVal.Value);
      var deviceAction = new Func<Task>(() => Device.Wait(duration));
      return deviceAction;
    }
  }
}
