using Ares.Messaging;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class DeviceCommandResultEntityConfiguration : AresEntityTypeBaseConfiguration<DeviceCommandResult>
{
  public override void Configure(EntityTypeBuilder<DeviceCommandResult> builder)
  {
    base.Configure(builder);
    builder.ToTable("DeviceCommandResults");

    builder.Property(result => result.Result)
      .HasConversion(s => s.ToByteArray(),
        bytes => ByteString.CopyFrom(bytes));
  }
}
