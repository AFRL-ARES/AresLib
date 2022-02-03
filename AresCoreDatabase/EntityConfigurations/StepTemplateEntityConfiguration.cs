using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.EFCore.EntityConfigurations;

internal class StepTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<StepTemplate>
{
  public override void Configure(EntityTypeBuilder<StepTemplate> builder)
  {
    base.Configure(builder);

    builder.HasMany(stepTemplate => stepTemplate.CommandTemplates)
      .WithOne()
      .IsRequired();

    builder.Navigation(stepTemplate => stepTemplate.CommandTemplates)
      .AutoInclude();
  }
}
