using System.Reflection;
using Ares.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore;

public class CoreDatabaseContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
{
  public CoreDatabaseContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<CampaignTemplate>? CampaignTemplates { get; set; }
  public DbSet<Project>? Projects { get; set; }
  public DbSet<StepTemplate>? StepTemplates { get; set; }
  public DbSet<ExperimentTemplate>? ExperimentTemplates { get; set; }
  public DbSet<PlannerTransaction>? PlannerTransactions { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    var assembly = Assembly.GetAssembly(typeof(CoreDatabaseContext<TUser>));
    if (assembly is null)
      return;

    modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    base.OnModelCreating(modelBuilder);
  }
}
