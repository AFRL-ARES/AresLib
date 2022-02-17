using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class ProjectEntityConfiguration : AresEntityTypeBaseConfiguration<Project>
{
  public override void Configure(EntityTypeBuilder<Project> builder)
  {
    base.Configure(builder);
    builder.HasMany(entity => entity.CompletedCampaigns)
           .WithOne();

    // TODO: figure out if deleting a project deletes campaign completed campaigns and stuff
    builder.Navigation(entity => entity.CompletedCampaigns)
           .AutoInclude();
  }
}