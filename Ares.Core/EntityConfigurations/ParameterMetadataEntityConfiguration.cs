﻿using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class ParameterMetadataEntityConfiguration : AresEntityTypeBaseConfiguration<ParameterMetadata>
{
  public override void Configure(EntityTypeBuilder<ParameterMetadata> builder)
  {
    base.Configure(builder);
    builder.HasMany(parameterMetadata => parameterMetadata.Constraints)
      .WithOne()
      .IsRequired();

    builder.Navigation(parameterMetadata => parameterMetadata.Constraints)
      .AutoInclude();
  }
}
