using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

class NotificationEntityConfiguration : AresEntityTypeBaseConfiguration<AresNotification>
{
  public override void Configure(EntityTypeBuilder<AresNotification> builder)
  {
    builder.ToTable("Notifications");
    base.Configure(builder);

    builder.Property(info => info.Timestamp)
      .HasConversion(timestamp => timestamp.ToDateTime(), time => time.ToTimestampUtc());
  }
}
