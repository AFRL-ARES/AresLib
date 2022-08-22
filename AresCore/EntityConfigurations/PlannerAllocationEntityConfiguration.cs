using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EntityConfigurations;

internal class PlannerAllocationEntityConfiguration : AresEntityTypeBaseConfiguration<PlannerAllocation>
{
  public override void Configure(EntityTypeBuilder<PlannerAllocation> builder)
  {
    base.Configure(builder);
    builder.ToTable("PlannerAllocations");

    builder.HasOne<PlannerInfo>()
      .WithMany();
  }
}
