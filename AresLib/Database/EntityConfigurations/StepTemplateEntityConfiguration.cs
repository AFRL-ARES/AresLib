using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AresLib.Database.EntityConfigurations
{
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
}
