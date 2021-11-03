using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class ExperimentTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<ExperimentTemplate>
  {
    public override void Configure(EntityTypeBuilder<ExperimentTemplate> builder)
    {
      base.Configure(builder);
      builder.HasMany(experimentTemplate => experimentTemplate.StepTemplates)
        .WithOne()
        .IsRequired();
      builder.Navigation(experimentTemplate => experimentTemplate.StepTemplates)
             .AutoInclude();

    }
  }
}
