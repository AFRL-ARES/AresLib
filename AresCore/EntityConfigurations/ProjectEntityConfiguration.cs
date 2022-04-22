using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class ProjectEntityConfiguration : AresEntityTypeBaseConfiguration<Project>
{
  public override void Configure(EntityTypeBuilder<Project> builder)
  {
    base.Configure(builder);
    builder.ToTable("Projects");
    builder.HasMany(entity => entity.CompletedCampaigns)
      .WithOne();

    builder.Navigation(entity => entity.CompletedCampaigns)
      .AutoInclude();
  }
}
