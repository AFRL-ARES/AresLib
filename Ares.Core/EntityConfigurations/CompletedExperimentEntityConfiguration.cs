using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CompletedExperimentEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedExperiment>
{
  public override void Configure(EntityTypeBuilder<CompletedExperiment> builder)
  {
    base.Configure(builder);
    builder.ToTable("CompletedExperiments");

    builder.HasOne(experiment => experiment.Template)
      .WithOne()
      .HasForeignKey<ExperimentTemplate>("CompletedExperimentId")
      .OnDelete(DeleteBehavior.ClientCascade);

    builder.HasMany(experiment => experiment.PlannerTransactions)
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(experiment => experiment.PlannerTransactions)
      .AutoInclude();

    builder.Navigation(experiment => experiment.Template)
      .AutoInclude();

    // TODO: revisit this one, might be broken at the moment
    //builder.HasOne(experiment => experiment.Result)
    //  .WithOne()
    //  .OnDelete(DeleteBehavior.ClientCascade);
  }
}
