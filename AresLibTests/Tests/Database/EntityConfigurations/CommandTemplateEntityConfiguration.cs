﻿using Ares.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class CommandTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CommandTemplate>
  {
    public override void Configure(EntityTypeBuilder<CommandTemplate> builder)
    {
      base.Configure(builder);
      builder.HasMany(commandTemplate => commandTemplate.Arguments)
        .WithOne()
        .IsRequired();
      builder.Navigation(commandTemplate => commandTemplate.Arguments)
             .AutoInclude();
      builder.Navigation(commandTemplate => commandTemplate.Metadata)
             .AutoInclude();
    }
  }
}
