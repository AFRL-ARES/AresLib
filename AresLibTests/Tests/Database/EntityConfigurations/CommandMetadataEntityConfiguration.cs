using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class CommandMetadataEntityConfiguration : AresEntityTypeBaseConfiguration<CommandMetadata>
  {
    public override void Configure(EntityTypeBuilder<CommandMetadata> builder)
    {
      base.Configure(builder);
      builder.HasMany(commandMetadata => commandMetadata.ParameterMetadatas)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);
      // TODO figure out how to deal with/remove metadata upon removal of a command template
      // the commented code below works only if there is a new instance of a CommandMetadata every time
      // it is used, otherwise the foreign key gets overwritten with every update
      builder.HasOne<CommandTemplate>()
             .WithOne(commandTemplate => commandTemplate.Metadata)
             .HasForeignKey<CommandMetadata>("CommandTemplateId")
             .IsRequired();

      builder.Navigation(commandMetadata => commandMetadata.ParameterMetadatas)
             .AutoInclude();
    }
  }
}
