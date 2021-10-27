using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal class ExperimentTemplateEntityConfiguration : AresEntityTypeBaseConfiguration<ExperimentTemplate>
  {
    public override void Configure(EntityTypeBuilder<ExperimentTemplate> builder)
    {
      base.Configure(builder);
      builder.Navigation(experimentTemplate => experimentTemplate.StepTemplates)
             .AutoInclude();

    }
  }
}
