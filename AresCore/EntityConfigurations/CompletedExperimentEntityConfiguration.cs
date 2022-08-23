using Ares.Messaging;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CompletedExperimentEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedExperiment>
{
  public override void Configure(EntityTypeBuilder<CompletedExperiment> builder)
  {
    base.Configure(builder);
    builder.ToTable("CompletedExperiments");
    builder.Navigation(experiment => experiment.Template)
      .AutoInclude();

    builder.HasMany(experiment => experiment.PlannerTransactions)
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(experiment => experiment.PlannerTransactions)
      .AutoInclude();

    builder.Property(experiment => experiment.SerializedData)
      .HasConversion(
        s => s.ToByteArray(),
        bytes => ByteString.CopyFrom(bytes));
  }
}
