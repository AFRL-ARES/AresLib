using Ares.Messaging;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class CompletedExperimentEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedExperiment>
{
  public override void Configure(EntityTypeBuilder<CompletedExperiment> builder)
  {
    base.Configure(builder);
    builder.ToTable("CompletedExperiments");
    builder.Navigation(experiment => experiment.Template)
      .AutoInclude();

    builder.Property(experiment => experiment.SerializedData)
      .HasConversion(
        s => s.ToByteArray(),
        bytes => ByteString.CopyFrom(bytes));

    builder.HasOne<CompletedCampaign>()
      .WithMany(campaign => campaign.Experiments);
  }
}
