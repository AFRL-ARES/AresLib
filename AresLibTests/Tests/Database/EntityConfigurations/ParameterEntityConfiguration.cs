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
  internal class ParameterEntityConfiguration : AresEntityTypeBaseConfiguration<Parameter>
  {
    public override void Configure(EntityTypeBuilder<Parameter> builder)
    {
      base.Configure(builder);
      builder.Navigation(parameter => parameter.Metadata)
             .AutoInclude();
    }
  }
}
