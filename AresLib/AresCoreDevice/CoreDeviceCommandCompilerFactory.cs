#nullable enable
using Ares.Core;
using AresLib.Compilers;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet.Units;

namespace AresLib.AresCoreDevice
{
  internal class CoreDeviceCommandCompilerFactory : DeviceCommandCompilerFactory<CoreDevice, CoreDeviceCommandType>
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


    protected override void ParseAndPerformDeviceAction(CoreDeviceCommandType deviceCommandEnum, CommandParameter[] commandParameters)
    {
      switch (deviceCommandEnum)
      {
        case CoreDeviceCommandType.Delay:
        case CoreDeviceCommandType.Wait:
          ParseAndCallWait(commandParameters[0]);
          break;
        default:
          throw new NotImplementedException($"Could not find function to parse parameters and route to {Device.Name}'s method for executing {deviceCommandEnum} at {GetType().Name}");
      }
    }

    private void ParseAndCallWait(CommandParameter durationParameter)
    {
      // This wouldn't be so naive in reality. We'd check the units and do FromMS, or FromHours, etc if needed
      var duration = TimeSpan.FromSeconds(durationParameter.Value);
      Device.Wait(duration).Wait();
    }
  }
}
