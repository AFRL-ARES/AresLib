﻿using System.Reflection;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core;

public class CoreDatabaseContext : DbContext
{
  public CoreDatabaseContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<CampaignTemplate> CampaignTemplates => Set<CampaignTemplate>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<StepTemplate> StepTemplates => Set<StepTemplate>();
  public DbSet<ExperimentTemplate> ExperimentTemplates => Set<ExperimentTemplate>();
  public DbSet<PlannerTransaction> PlannerTransactions => Set<PlannerTransaction>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    var assembly = Assembly.GetAssembly(typeof(CoreDatabaseContext));
    if (assembly is null)
      return;

    modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    base.OnModelCreating(modelBuilder);
  }
}
