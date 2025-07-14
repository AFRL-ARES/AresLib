using Ares.Messaging;

namespace Ares.Tools;
public static class AresStructHelper
{
  public static void AddString(this AresStruct aresStruct, string key, string value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateString(value);
  }

  public static void AddNumber(this AresStruct aresStruct, string key, int value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumber(value);
  }

  public static void AddNumber(this AresStruct aresStruct, string key, double value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumber(value);
  }

  public static void AddNumber(this AresStruct aresStruct, string key, float value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumber(value);
  }

  public static void AddNull(this AresStruct aresStruct, string key)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNull();
  }

  public static void AddStringArray(this AresStruct aresStruct, string key, IEnumerable<string> values)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateStringArray(values);
  }

  public static void AddNumberArray(this AresStruct aresStruct, string key, IEnumerable<int> values)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
  }

  public static void AddNumberArray(this AresStruct aresStruct, string key, IEnumerable<double> values)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
  }

  public static void AddNumberArray(this AresStruct aresStruct, string key, IEnumerable<float> values)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
  }

  public static void AddBool(this AresStruct aresStruct, string key, bool value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateBool(value);
  }

  public static void AppendStruct(this AresStruct aresStruct, AresStruct otherStruct)
  {
    foreach(var field in otherStruct.Fields)
    {
      aresStruct.Fields[field.Key] = field.Value;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="aresStruct"></param>
  /// <param name="key"></param>
  /// <param name="value"></param>
  /// <param name="replace"></param>
  /// <exception cref="ArgumentException">Thrown when the value already exists if <param name="replace"></param> is set to 'false'</exception>
  public static void AddValue(this AresStruct aresStruct, string key, AresValue value, bool replace = true)
  {
    if(replace)
    {
      aresStruct.Fields[key] = value.Clone();
    }
    else
    {
      aresStruct.Fields.Add(key, value.Clone());
    }
  }
}
