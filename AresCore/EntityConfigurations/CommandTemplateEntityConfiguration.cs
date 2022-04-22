using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class CommandTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CommandTemplate>
{
  public override void Configure(EntityTypeBuilder<CommandTemplate> builder)
  {
    base.Configure(builder);
    builder.ToTable("CommandTemplates");
    builder.HasMany(commandTemplate => commandTemplate.Arguments)
      .WithOne()
      .IsRequired(false);

    builder.HasOne(template => template.Metadata)
      .WithOne()
      .HasForeignKey<CommandMetadata>("CommandTemplateId");

    builder.Navigation(commandTemplate => commandTemplate.Arguments)
      .AutoInclude();

    builder.Navigation(commandTemplate => commandTemplate.Metadata)
      .AutoInclude();
  }
}
