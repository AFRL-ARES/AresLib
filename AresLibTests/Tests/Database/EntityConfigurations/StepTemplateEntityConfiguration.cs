using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class StepTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<StepTemplate>
  {
    public override void Configure(EntityTypeBuilder<StepTemplate> builder)
    {
      base.Configure(builder);
      builder.Navigation(stepTemplate => stepTemplate.CommandTemplates)
             .AutoInclude();
    }
  }
}
