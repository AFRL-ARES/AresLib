using Ares.Core.Messages;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLib.Database.EntityConfigurations
{
  class CompletedExperimentEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedExperiment>
  {
    public override void Configure(EntityTypeBuilder<CompletedExperiment> builder)
    {
      base.Configure(builder);
      builder.Navigation(experiment => experiment.Template)
        .AutoInclude();

      builder.Property(experiment => experiment.SerializedData)
        .HasConversion(
          s => s.ToByteArray(),
          bytes => ByteString.CopyFrom(bytes));
    }
  }
}
