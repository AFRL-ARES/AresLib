using Ares.Device;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using UnitsNet;
using UnitsNet.Units;

namespace CoreDevice;

public class AresCoreDeviceCommandInterpreter : DeviceCommandInterpreter<AresCoreDevice, AresCoreDeviceCommand>
{
  public AresCoreDeviceCommandInterpreter(AresCoreDevice device) : base(device)
  { }

  protected override CommandMetadata[] CommandsToMetadatas()
  {
    return new CommandMetadata[]
    {
      new CommandMetadata
      {
        DeviceName = Device.Name,
        Name = AresCoreDeviceCommand.Sleep.ToString(),
        Description = "Sleep for a given amount of time.",
        ParameterMetadatas =
          {
            new ParameterMetadata
            {
              Name = AresCoreDeviceCommandParameter.Duration.ToString(),
              Index = 0,
              Unit = $"{DurationUnit.Millisecond}s"
            }
          }
      }
    };
  }

  protected override async Task<DeviceCommandResult> ParseAndPerformDeviceAction(AresCoreDeviceCommand deviceCommandEnum, Parameter[] parameters, CancellationToken cancellationToken)
  {
    var result = new DeviceCommandResult();
    switch(deviceCommandEnum)
    {
      case AresCoreDeviceCommand.Sleep:
        var durationParam = parameters[0];
        var unpacked = durationParam.Value.Value.TryUnpack<StringValue>(out var stringValueParam);
        var parsed = double.TryParse(stringValueParam.Value, out double doubleParam);

        if(!unpacked || !parsed)
        {
          result.Success = false;
          result.Error = $"Failed to parse command argument into valid sleep time value, ARES could not sleep!";
          return result;
        }

        var duration = UnitsNet.Duration.FromMilliseconds(doubleParam);
        await Device.Sleep(duration.ToTimeSpan());
        result.Success = true;
        return result;

      default:
        throw new ArgumentOutOfRangeException(nameof(deviceCommandEnum), deviceCommandEnum, null);
    }
  }
}
