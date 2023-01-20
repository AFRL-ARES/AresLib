using Ares.Messaging.Device;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;
internal class DeviceStateLogEntityConfiguration : AresEntityTypeBaseConfiguration<DeviceStateLog>
{
  public override void Configure(EntityTypeBuilder<DeviceStateLog> builder)
  {
    base.Configure(builder);
    builder.ToTable("DeviceStateLogs");

    builder.Navigation(config => config.StateData).AutoInclude();

    builder.Property(state => state.Timestamp)
      .HasConversion(timestamp => timestamp.ToDateTime(), time => time.ToTimestampUtc());

    builder.HasOne(config => config.StateData)
      .WithOne()
      .HasForeignKey<Any>("DeviceStateLogId")
      .OnDelete(DeleteBehavior.ClientCascade);
  }
}
