using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class CommandMetadataEntityConfiguration : AresEntityTypeBaseConfiguration<CommandMetadata>
  {
    public override void Configure(EntityTypeBuilder<CommandMetadata> builder)
    {
      base.Configure(builder);
      builder.Navigation(commandMetadata => commandMetadata.ParameterMetadatas)
             .AutoInclude();
    }
  }
}
