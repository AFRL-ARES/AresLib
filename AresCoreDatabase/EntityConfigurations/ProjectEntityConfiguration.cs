using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresCoreDatabase.EntityConfigurations;

internal class ProjectEntityConfiguration : AresEntityTypeBaseConfiguration<Project>
{
  public override void Configure(EntityTypeBuilder<Project> builder)
  {
    base.Configure(builder);
    builder.HasMany(entity => entity.CampaignTemplates)
      .WithOne();

    builder.HasMany(entity => entity.CompletedExperiments)
      .WithOne();

    // TODO: figure out if deleting a project deleted campaign templates and stuff
    builder.Navigation(entity => entity.CampaignTemplates)
      .AutoInclude();

    // .HasField("campaignTemplates_");
    builder.Navigation(entity => entity.CompletedExperiments)
      .AutoInclude();
    // .HasField("completedExperiments_");
  }
}
