using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.EFCore.EntityConfigurations;

internal abstract class AresEntityTypeBaseConfiguration<TAresCoreEntity> : IEntityTypeConfiguration<TAresCoreEntity> where TAresCoreEntity : class, IMessage
{
  public virtual void Configure(EntityTypeBuilder<TAresCoreEntity> builder)
  {
    builder
      .Property<Guid>("UniqueId")
      .ValueGeneratedOnAdd();

    builder
      .Property<DateTime>("CreationTime")
      .ValueGeneratedOnAdd()
      .HasDefaultValue(DateTime.UtcNow);

    builder
      .Property<DateTime>("LastModified")
      .ValueGeneratedOnUpdate()
      .HasDefaultValue(DateTime.UtcNow);

    builder.HasKey("UniqueId");
  }
}
