using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CommandResultEntityConfiguration : AresEntityTypeBaseConfiguration<CommandResult>
{
  public override void Configure(EntityTypeBuilder<CommandResult> builder)
  {
    base.Configure(builder);
    builder.ToTable("CommandResults");
    builder.HasOne(result => result.ExecutionInfo)
      .WithOne()
      .HasForeignKey<ExecutionInfo>("CommandResultId")
      .OnDelete(DeleteBehavior.ClientCascade);
  }
}
