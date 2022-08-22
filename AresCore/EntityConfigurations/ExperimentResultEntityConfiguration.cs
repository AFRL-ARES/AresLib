using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class ExperimentResultEntityConfiguration : AresEntityTypeBaseConfiguration<ExperimentResult>
{
  public override void Configure(EntityTypeBuilder<ExperimentResult> builder)
  {
    base.Configure(builder);
    builder.ToTable("ExperimentResults");
    builder.HasMany<StepResult>()
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<CompletedExperiment>()
      .WithOne()
      .HasForeignKey<CompletedExperiment>("ExperimentResultId");
    
    builder.HasOne<ExecutionInfo>()
      .WithOne()
      .HasForeignKey<ExecutionInfo>("ExperimentResultId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
