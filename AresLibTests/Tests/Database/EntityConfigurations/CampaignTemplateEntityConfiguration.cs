using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class CampaignTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CampaignTemplate>
  {
    public override void Configure(EntityTypeBuilder<CampaignTemplate> builder)
    {
      base.Configure(builder);
      builder.Navigation(campaignTemplate => campaignTemplate.ExperimentTemplates)
             .AutoInclude();
    }
  }
}
