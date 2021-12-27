﻿using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresCoreDatabase.EntityConfigurations;

internal class CampaignTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CampaignTemplate>
{
  public override void Configure(EntityTypeBuilder<CampaignTemplate> builder)
  {
    base.Configure(builder);
    builder.HasMany(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .WithOne()
      .IsRequired();

    builder.Navigation(campaignTemplate => campaignTemplate.ExperimentTemplates)
      .AutoInclude();
  }
}
