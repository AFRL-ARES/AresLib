using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Ares.Core.EntityConfigurations.Helpers;

public static class TextTypeDeterminationHelper
{
  public static string DetermineColumnType()
  {
    var provider = DatabaseRuntimeEnvironment.DatabaseProvider;

    if(provider is null)
      return "TEXT";

    if(provider.Contains("Postgres", StringComparison.CurrentCultureIgnoreCase))
      return "jsonb";

    if(provider.Contains("Sqlite", StringComparison.CurrentCultureIgnoreCase))
      return "TEXT";

    if(provider.Contains("SqlServer", StringComparison.CurrentCultureIgnoreCase))
      return "nvarchar(max)";

    else
      return "TEXT";
  }

  public static PropertyBuilder<AresValue> HasAresValue(this PropertyBuilder<AresValue> value)
  {
    return value.HasConversion(
        v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
        v => JsonSerializer.Deserialize<AresValue>(v, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new AresValue())
      .HasColumnType(DetermineColumnType());
  }

  public static PropertyBuilder<AresDataSchemaSimplified> HasDataSchemaSimplified(this PropertyBuilder<AresDataSchemaSimplified> schema)
  {
    return schema.HasConversion(
      s => JsonSerializer.Serialize(s, new JsonSerializerOptions(JsonSerializerDefaults.General)),
      s => JsonSerializer.Deserialize<AresDataSchemaSimplified>(s, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new AresDataSchemaSimplified())
      .HasColumnType(DetermineColumnType());
  }

  public static PropertyBuilder<AresStruct> HasAresStruct(this PropertyBuilder<AresStruct> aresStruct)
  {
    return aresStruct.HasConversion(
      s => JsonSerializer.Serialize(s, new JsonSerializerOptions(JsonSerializerDefaults.General)),
      s => JsonSerializer.Deserialize<AresStruct>(s, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new AresStruct())
      .HasColumnType(DetermineColumnType());
  }
}
