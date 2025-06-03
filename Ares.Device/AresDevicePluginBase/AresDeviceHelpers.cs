using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Device;

public static class AresDeviceHelpers
{
  public static DeviceCommandResult ParseCommandParameterToInt(Parameter param, out int parsedParam)
  {
    var result = new DeviceCommandResult();
    var unpacked = param.Value.Value.TryUnpack<StringValue>(out var paramString);
    var parsed = int.TryParse(paramString.Value, out var intParam);

    if(!unpacked || !parsed)
    {
      result.Error = $"Failed to parse {param.Metadata.Name} into integer!";
      result.Success = false;
      parsedParam = -1;
      return result;
    }

    else
    {
      parsedParam = intParam;
      result.Success = true;
    }

    return result;
  }

  public static DeviceCommandResult ParseCommandParameterToDouble(Parameter param, out double parsedParam)
  {
    var result = new DeviceCommandResult();
    var unpacked = param.Value.Value.TryUnpack<StringValue>(out var paramStringValue);
    var parsed = double.TryParse(paramStringValue.Value, out var doubleParam);

    if(!unpacked || !parsed)
    {
      result.Error = $"Failed to parse {param.Metadata.Name} into double!";
      result.Success = false;
      parsedParam = -1;
      return result;
    }

    else
    {
      parsedParam = doubleParam;
      result.Success = true;
    }

    return result;
  }
}
