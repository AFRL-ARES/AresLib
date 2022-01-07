using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresCoreDatabase.EntityConfigurations;

internal class ParameterMetadataEntityConfiguration : AresEntityTypeBaseConfiguration<ParameterMetadata>
{
  public override void Configure(EntityTypeBuilder<ParameterMetadata> builder)
  {
    base.Configure(builder);
    builder.HasMany(parameterMetadata => parameterMetadata.Constraints)
      .WithOne()
      .IsRequired();

    builder.HasOne<Parameter>()
      .WithOne(parameter => parameter.Metadata)
      .HasForeignKey<ParameterMetadata>("ParameterId")
      .OnDelete(DeleteBehavior.NoAction);
    // TODO figure out how to cleanup metadata, see CommandMetadataEntityConfiguration for details

    builder.Navigation(parameterMetadata => parameterMetadata.Constraints)
      .AutoInclude();
  }
}
