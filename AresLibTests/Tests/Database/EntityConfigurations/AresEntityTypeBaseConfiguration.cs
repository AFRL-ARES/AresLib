using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AresLibTests.Tests.Database.EntityConfigurations
{
  internal abstract class AresEntityTypeBaseConfiguration<AresCoreEntity> : IEntityTypeConfiguration<AresCoreEntity> where AresCoreEntity : class, IMessage
  {
    public virtual void Configure(EntityTypeBuilder<AresCoreEntity> builder)
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
