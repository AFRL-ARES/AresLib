using System.Text.Json;
using Ares.Messaging;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;
internal class AresStructEntityConfiguration : AresEntityTypeBaseConfiguration<AresStruct>
{
  public override void Configure(EntityTypeBuilder<AresStruct> builder)
  {
    builder.Property(a => a.Fields)
        .HasConversion(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions(JsonSerializerDefaults.General)),
            v => JsonSerializer.Deserialize<MapField<string, AresValue>>(v, new JsonSerializerOptions(JsonSerializerDefaults.General)) ?? new MapField<string, AresValue>()
        )
        .HasColumnType(DetermineTableType());
  }

  private static string DetermineTableType()
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
}
