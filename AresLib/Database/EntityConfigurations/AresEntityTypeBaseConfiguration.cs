using System;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace AresLib.Database.EntityConfigurations
{
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
        .HasDefaultValue(DateTime.Now);
      builder
        .Property<DateTime>("LastModified")
        .ValueGeneratedOnUpdate()
        .HasDefaultValue(DateTime.Now);
      builder.HasKey("UniqueId");
    }
  }
}
