using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class StepResultEntityConfiguration : AresEntityTypeBaseConfiguration<StepResult>
{
  public override void Configure(EntityTypeBuilder<StepResult> builder)
  {
    base.Configure(builder);
    builder.ToTable("StepResults");
    builder.HasMany<CommandResult>()
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne<ExecutionInfo>()
      .WithOne()
      .HasForeignKey<ExecutionInfo>("StepResultId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
