using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class CommandTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<CommandTemplate>
  {
    public override void Configure(EntityTypeBuilder<CommandTemplate> builder)
    {
      base.Configure(builder);
      builder.Navigation(commandTemplate => commandTemplate.Arguments)
             .AutoInclude();
      builder.Navigation(commandTemplate => commandTemplate.Metadata)
             .AutoInclude();
    }
  }
}
