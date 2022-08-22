using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class ExperimentExecutionStatusEntityConfiguration : AresEntityTypeBaseConfiguration<ExperimentExecutionStatus>
{
  public override void Configure(EntityTypeBuilder<ExperimentExecutionStatus> builder)
  {
    base.Configure(builder);
    builder.ToTable("ExperimentExecutionStatuses");

    builder.HasMany<StepExecutionStatus>()
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);
  }
}
