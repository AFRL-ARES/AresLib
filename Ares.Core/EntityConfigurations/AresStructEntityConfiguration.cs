using Ares.Core.EntityConfigurations.Helpers;
using Ares.Messaging;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

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
        .HasColumnType(TextTypeDeterminationHelper.DetermineColumnType());
  }
}
