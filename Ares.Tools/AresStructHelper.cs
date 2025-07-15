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

  public static void AddBytes(this AresStruct aresStruct, string key, byte[] value)
  {
    aresStruct.Fields[key] = AresValueHelper.CreateBytes(value);
  }

  public static AresStruct CreateStringStruct(string key, string value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateString(value);
    return newStruct;
  }

  public static AresStruct CreateNumberStruct(string key, int value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumber(value);
    return newStruct;
  }

  public static AresStruct CreateNumberStruct(string key, double value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumber(value);
    return newStruct;
  }

  public static AresStruct CreateNumberStruct(string key, float value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumber(value);
    return newStruct;
  }

  public static AresStruct CreateNullStruct(string key)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNull();
    return newStruct;
  }

  public static AresStruct CreateStringArrayStruct(string key, IEnumerable<string> values)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateStringArray(values);
    return newStruct;
  }

  public static AresStruct CreateNumberArrayStruct(string key, IEnumerable<int> values)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
    return newStruct;
  }

  public static AresStruct CreateNumberArrayStruct(string key, IEnumerable<double> values)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
    return newStruct;
  }

  public static AresStruct CreateNumberArrayStruct(string key, IEnumerable<float> values)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateNumberArray(values);
    return newStruct;
  }

  public static AresStruct CreateBoolStruct(string key, bool value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateBool(value);
    return newStruct;
  }

  public static AresStruct CreateStructAndAppend(AresStruct otherStruct)
  {
    AresStruct newStruct = new AresStruct();
    foreach(var field in otherStruct.Fields)
    {
      newStruct.Fields[field.Key] = field.Value;
    }
    return newStruct;
  }

  public static AresStruct CreateBytesStruct(string key, byte[] value)
  {
    AresStruct newStruct = new AresStruct();
    newStruct.Fields[key] = AresValueHelper.CreateBytes(value);
    return newStruct;
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
