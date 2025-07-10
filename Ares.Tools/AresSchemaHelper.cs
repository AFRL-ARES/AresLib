using Ares.Messaging;

namespace Ares.Tools;
public static class AresSchemaHelper
{
  public static void AddEntry(this AresDataSchema schema, string name, AresDataType type, bool optional)
  {
    var entry = new SchemaEntry() { Type = type, Optional = optional };
    schema.Fields[name] = entry;
  }

  public static void AddEntry(this AresDataSchema schema, string name, AresDataType type, bool optional, IEnumerable<string> stringOptions)
  {
    var entry = new SchemaEntry() { Type = type, Optional = optional };
    if(type != AresDataType.String && type != AresDataType.StringList)
    {
      throw new InvalidOperationException($"Cannot provide string options to a datatype that is {type}");
    }
    entry.StringChoices = new StringList();
    entry.StringChoices.Strings.AddRange(stringOptions);
    schema.Fields[name] = entry;
  }

  public static void AddEntry(this AresDataSchema schema, string name, AresDataType type, bool optional, IEnumerable<double> numOptions)
  {
    var entry = new SchemaEntry() { Type = type, Optional = optional };
    if(type != AresDataType.Number && type != AresDataType.NumberList)
    {
      throw new InvalidOperationException($"Cannot provide number options to a datatype that is {type}");
    }
    entry.NumberChoices = new NumberList();
    entry.NumberChoices.Numbers.AddRange(numOptions);
    schema.Fields[name] = entry;
  }

  public static void AddEntry(this AresDataSchema schema, string name, AresDataType type, bool optional, IEnumerable<int> numOptions)
  {
    schema.AddEntry(name, type, optional, numOptions.Select(n => (double)n));
  }

  public static void AddEntry(this AresDataSchema schema, string name, AresDataType type, bool optional, IEnumerable<float> numOptions)
  {
    schema.AddEntry(name, type, optional, numOptions.Select(n => (double)n));
  }
}
