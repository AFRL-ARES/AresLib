using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

public class CompletedCampaignEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedCampaign>
{
  public override void Configure(EntityTypeBuilder<CompletedCampaign> builder)
  {
    base.Configure(builder);
    builder.Navigation(campaign => campaign.Experiments)
      .AutoInclude();

    builder.Navigation(campaign => campaign.PlannerTransactions)
      .AutoInclude();

    builder.Navigation(campaign => campaign.Template)
      .AutoInclude();
  }
}
