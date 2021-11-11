using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AresLib.Database.EntityConfigurations
{
  internal class ExperimentTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<ExperimentTemplate>
  {
    public override void Configure(EntityTypeBuilder<ExperimentTemplate> builder)
    {
      base.Configure(builder);
      builder.HasMany(experimentTemplate => experimentTemplate.StepTemplates)
        .WithOne()
        .IsRequired();
      builder.HasMany<CompletedExperiment>()
        .WithOne(experiment => experiment.Template)
        .IsRequired(); // TODO decide whether deleting a completed experiment requires there to be an experiment template
      builder.Navigation(experimentTemplate => experimentTemplate.StepTemplates)
             .AutoInclude();

    }
  }
}
