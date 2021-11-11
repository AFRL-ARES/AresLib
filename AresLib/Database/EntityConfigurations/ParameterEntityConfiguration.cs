using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AresLib.Database.EntityConfigurations
{
  internal class ParameterEntityConfiguration : AresEntityTypeBaseConfiguration<Parameter>
  {
    public override void Configure(EntityTypeBuilder<Parameter> builder)
    {
      base.Configure(builder);
      builder.Navigation(parameter => parameter.Metadata)
             .AutoInclude();
      builder.HasOne<ParameterMetadata>()
             .WithOne();
    }
  }
}
