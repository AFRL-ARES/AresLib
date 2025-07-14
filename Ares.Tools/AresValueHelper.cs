using Ares.Messaging;
using Google.Protobuf;

namespace Ares.Tools;
public static class AresValueHelper
{
  public static AresValue CreateNumber(int value)
  {
    return new AresValue { NumberValue = value, };
  }

  public static AresValue CreateNumber(double value)
  {
    return new AresValue { NumberValue = value, };
  }

  public static AresValue CreateNumber(float value)
  {
    return new AresValue { NumberValue = value, };
  }

  public static AresValue CreateString(string value)
  {
    return new AresValue { StringValue = value, };
  }

  public static AresValue CreateNumberArray(IEnumerable<int> values)
  {
    var val = new AresValue { NumberArrayValue = new NumberArray() };
    val.NumberArrayValue.Numbers.AddRange(values.Select(num => (double)num));

    return val;
  }

  public static AresValue CreateNumberArray(IEnumerable<double> values)
  {
    var val = new AresValue { NumberArrayValue = new NumberArray() };
    val.NumberArrayValue.Numbers.AddRange(values.Select(num => num));

    return val;
  }

  public static AresValue CreateNumberArray(IEnumerable<float> values)
  {
    var val = new AresValue { NumberArrayValue = new NumberArray() };
    val.NumberArrayValue.Numbers.AddRange(values.Select(num => (double)num));

    return val;
  }

  public static AresValue CreateStringArray(IEnumerable<string> values)
  {
    var val = new AresValue { StringArrayValue = new StringArray() };
    val.StringArrayValue.Strings.AddRange(values);

    return val;
  }

  public static AresValue CreateNull()
  {
    return new AresValue { NullValue = NullValue.NullValue };
  }

  public static AresValue CreateBool(bool value)
  {
    return new AresValue { BoolValue = value };
  }

  public static AresValue CreateBytes(byte[] bytes)
  {
    return new AresValue { BytesValue = ByteString.CopyFrom(bytes) };
  }

  public static AresValue CreateBoolArray(IEnumerable<bool> values)
  {
    var val = new AresValue { BoolArrayValue = new BoolArray() };
    val.BoolArrayValue.Bools.AddRange(values);

    return val;
  }
}
