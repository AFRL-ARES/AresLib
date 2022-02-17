﻿using Ares.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ares.Core.EFCore.EntityConfigurations;

internal class PlannerTransactionEntityConfiguration : AresEntityTypeBaseConfiguration<PlannerTransaction>
{
  public override void Configure(EntityTypeBuilder<PlannerTransaction> builder)
  {
    base.Configure(builder);

    builder.Navigation(transaction => transaction.Request).AutoInclude();
    builder.Navigation(transaction => transaction.Response).AutoInclude();

    builder.HasOne<CompletedCampaign>().WithMany(campaign => campaign.PlannerTransactions);
  }
}