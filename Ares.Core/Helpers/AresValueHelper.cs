using Ares.Messaging;
using Google.Protobuf;

namespace Ares.Core.Helpers;
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

  public static AresValue CreateNumberList(IEnumerable<int> values)
  {
    var val = new AresValue { NumberListValue = new NumberList() };
    val.NumberListValue.Numbers.AddRange(values.Select(num => (double)num));

    return val;
  }

  public static AresValue CreateNumberList(IEnumerable<double> values)
  {
    var val = new AresValue { NumberListValue = new NumberList() };
    val.NumberListValue.Numbers.AddRange(values.Select(num => num));

    return val;
  }

  public static AresValue CreateNumberList(IEnumerable<float> values)
  {
    var val = new AresValue { NumberListValue = new NumberList() };
    val.NumberListValue.Numbers.AddRange(values.Select(num => (double)num));

    return val;
  }

  public static AresValue CreateStringList(IEnumerable<string> values)
  {
    var val = new AresValue { StringListValue = new StringList() };
    val.StringListValue.Strings.AddRange(values);

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
}
