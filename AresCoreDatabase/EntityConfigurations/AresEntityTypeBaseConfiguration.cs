using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

public abstract class AresEntityTypeBaseConfiguration<TAresCoreEntity> : IEntityTypeConfiguration<TAresCoreEntity> where TAresCoreEntity : class, IMessage
{
  public virtual void Configure(EntityTypeBuilder<TAresCoreEntity> builder)
  {
    const string dateGetterFunctionSql = "getdate()";

    builder
      .Property<Guid>("UniqueId")
      .ValueGeneratedOnAdd();

    builder
      .Property<DateTime>("CreationTime")
      .ValueGeneratedOnAdd()
      .HasDefaultValueSql(dateGetterFunctionSql);

    builder
      .Property<DateTime>("LastModified")
      .ValueGeneratedOnUpdate()
      .HasDefaultValueSql(dateGetterFunctionSql);

    builder.HasKey("UniqueId");
  }
}
