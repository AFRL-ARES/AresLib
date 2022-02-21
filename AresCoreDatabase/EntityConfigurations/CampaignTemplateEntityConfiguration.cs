using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class CampaignTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CampaignTemplate>
{
  public override void Configure(EntityTypeBuilder<CampaignTemplate> builder)
  {
    base.Configure(builder);
    builder.ToTable("CampaignTemplates");
    builder.HasMany(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .WithOne()
      .IsRequired();

    builder.HasMany(campaignTemplate => campaignTemplate.Planners)
      .WithOne()
      .IsRequired();// remove requirement if planners should exist separately from campaign templates

    builder.HasMany(campaignTemplate => campaignTemplate.PlannableParameters)
      .WithOne()
      .OnDelete(DeleteBehavior.ClientCascade)
      .IsRequired(false);

    builder.Navigation(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .AutoInclude();

    builder.Navigation(template => template.PlannableParameters)
      .AutoInclude();

    builder.Navigation(template => template.Planners)
      .AutoInclude();
  }
}
