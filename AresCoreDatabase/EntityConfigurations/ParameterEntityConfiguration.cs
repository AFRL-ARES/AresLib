using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class ParameterEntityConfiguration : AresEntityTypeBaseConfiguration<Parameter>
{
  public override void Configure(EntityTypeBuilder<Parameter> builder)
  {
    base.Configure(builder);
    builder.ToTable("Parameters");

    builder.HasOne(parameter => parameter.Metadata)
      .WithOne()
      .HasForeignKey<ParameterMetadata>("ParameterId")
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(parameter => parameter.Metadata)
      .AutoInclude();
  }
}
