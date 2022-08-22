using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

public class CompletedCampaignEntityConfiguration : AresEntityTypeBaseConfiguration<CompletedCampaign>
{
  public override void Configure(EntityTypeBuilder<CompletedCampaign> builder)
  {
    base.Configure(builder);
    builder.ToTable("CompletedCampaigns");
    builder.Navigation(campaign => campaign.Experiments)
      .AutoInclude();

    builder.Navigation(campaign => campaign.Template)
      .AutoInclude();

    builder.HasOne(campaign => campaign.Template)
      .WithMany();

    builder.HasMany(fd => fd.Experiments)
      .WithOne()
      .IsRequired();
  }
}
