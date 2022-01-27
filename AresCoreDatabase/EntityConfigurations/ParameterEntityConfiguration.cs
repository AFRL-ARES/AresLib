using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.EFCore.EntityConfigurations;

internal class ParameterEntityConfiguration : AresEntityTypeBaseConfiguration<Parameter>
{
  public override void Configure(EntityTypeBuilder<Parameter> builder)
  {
    base.Configure(builder);
    builder.Navigation(parameter => parameter.Metadata)
      .AutoInclude();
  }
}
