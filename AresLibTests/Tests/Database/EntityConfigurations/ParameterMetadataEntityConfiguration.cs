using Ares.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class ParameterMetadataEntityConfiguration : AresEntityTypeBaseConfiguration<ParameterMetadata>
  {
    public override void Configure(EntityTypeBuilder<ParameterMetadata> builder)
    {
      base.Configure(builder);
      builder.HasMany(parameterMetadata => parameterMetadata.Constraints)
        .WithOne()
        .IsRequired();
      // TODO figure out how to cleanup metadata, see CampaignMetadataEntityConfiguration for details
      builder.Navigation(parameterMetada => parameterMetada.Constraints)
             .AutoInclude();
    }
  }
}
