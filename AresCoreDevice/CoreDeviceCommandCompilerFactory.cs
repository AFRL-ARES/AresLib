using Ares.Core;
using System;
using System.Linq;
using DynamicData;
using UnitsNet;
using UnitsNet.Units;

namespace AresLib.AresCoreDevice
{
  internal class CoreDeviceCommandCompilerFactory : DeviceCommandCompilerFactory<CoreDevice, CoreDeviceCommandType>
  {
    public CoreDeviceCommandCompilerFactory()
    {

    }

    public void RegisterCommands()
    {
      var enums = 
        Enum.GetValues<CoreDeviceCommandType>()
          .ToArray();
    }


    public CommandMetadata WaitCommandMetadata { get; init; }

    protected CommandParameterMetadata GenerateParameterMetadata(string parameterName, Enum unit)
    {
      var parameterMetadata =
        new CommandParameterMetadata
        {
          Name = $"{parameterName}",
          Unit = $"{unit}"
        };

      return parameterMetadata;
    }

    protected void AddOrUpdateCommandMetadata(CoreDeviceCommandType name, string description)
    {

      var durationParameter = GenerateParameterMetadata("Duration", DurationUnit.Second);
      var commandMetadata =
        new CommandMetadata
        {
          DeviceName = $"{Device.Name}",
          Name = $"{name}",
          Description = description,
          ParameterMetadatas =
          {
            durationParameter
          }
        };

    }


    protected override Action GetDeviceAction(CommandTemplate commandTemplate)
    {
      if (commandTemplate.Metadata.Name.Equals("WAIT"))
      {
        var duration = TimeSpan.FromSeconds(0);
        var deviceAction = new Action(() => Device.Wait(duration));
        return deviceAction;
      }

      return null;
    }
  }
}
