using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class CampaignTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CampaignTemplate>
{
  public override void Configure(EntityTypeBuilder<CampaignTemplate> builder)
  {
    base.Configure(builder);
    builder.HasMany(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .WithOne()
      .IsRequired();

    builder.HasOne<CompletedCampaign>()
      .WithOne(campaign => campaign.Template)
      .HasForeignKey<CampaignTemplate>("CampaignId")
      .OnDelete(DeleteBehavior.NoAction);

    builder.Navigation(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .AutoInclude();
  }
}
