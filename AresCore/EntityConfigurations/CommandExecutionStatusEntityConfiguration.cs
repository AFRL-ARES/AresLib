using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CommandExecutionStatusEntityConfiguration : AresEntityTypeBaseConfiguration<CommandExecutionStatus>
{
  public override void Configure(EntityTypeBuilder<CommandExecutionStatus> builder)
  {
    base.Configure(builder);
    builder.ToTable("CommandExecutionStatuses");

    builder.HasOne<DeviceCommandResult>()
      .WithOne()
      .HasForeignKey<DeviceCommandResult>("CommandExecutionStatusId")
      .IsRequired();

    builder.Property(status => status.State)
      .HasConversion<string>();
  }
}