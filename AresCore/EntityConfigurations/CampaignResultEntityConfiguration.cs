using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CampaignResultEntityConfiguration : AresEntityTypeBaseConfiguration<CampaignResult>
{
  public override void Configure(EntityTypeBuilder<CampaignResult> builder)
  {
    base.Configure(builder);
    builder.ToTable("CampaignResults");
    builder.HasMany<ExperimentResult>()
      .WithOne()
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne<CompletedCampaign>()
      .WithOne()
      .HasForeignKey<CompletedCampaign>("ExperimentResultId");

    builder.HasOne<ExecutionInfo>()
      .WithOne()
      .HasForeignKey<ExecutionInfo>("ExperimentResultId")
      .OnDelete(DeleteBehavior.Cascade);
  }
}
