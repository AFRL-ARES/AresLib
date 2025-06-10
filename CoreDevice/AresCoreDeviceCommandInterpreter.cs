using Ares.Device;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
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
      },

      new CommandMetadata
      {
        DeviceName = Device.Name,
        Name = AresCoreDeviceCommand.WaitForUser.ToString(),
        Description = "ARES will request user confirmation before continuing.",
        ParameterMetadatas =
        {
          new ParameterMetadata
          {
            Name = AresCoreDeviceCommandParameter.ConfirmationMessage.ToString(),
            Index = 0
          }
        }
      }
    };
  }

  protected override async Task<DeviceCommandResult> ParseAndPerformDeviceAction(AresCoreDeviceCommand deviceCommandEnum, Parameter[] parameters, CommandMetadata metadata, CancellationToken cancellationToken)
  {
    var result = new DeviceCommandResult();
    switch(deviceCommandEnum)
    {
      case AresCoreDeviceCommand.Sleep:
        var durationParam = parameters[0];
        var parseResult = AresDeviceHelpers.ParseCommandParameterToDouble(durationParam, out var doubleValue);

        if(!parseResult.Success)
          return parseResult;

        var duration = UnitsNet.Duration.FromMilliseconds(doubleValue);
        await Device.Sleep(duration.ToTimeSpan());
        result.Success = true;
        return result;

      case AresCoreDeviceCommand.WaitForUser:
        var messageParam = parameters[0];
        var unpacked = messageParam.Value.Value.TryUnpack<StringValue>(out var stringValue);

        if(!unpacked)
        {
          result.Success = false;
          result.Error = "Failed to unpack message parameter in WaitForUser command!";
          return result;
        }

        await Device.WaitForUser(stringValue.Value);
        result.Success = true;
        return result;

      default:
        throw new ArgumentOutOfRangeException(nameof(deviceCommandEnum), deviceCommandEnum, null);
    }
  }
}
