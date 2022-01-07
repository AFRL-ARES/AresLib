using System.Reflection;
using Ares.Core.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AresCoreDatabase;

public class CoreDatabaseContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
{
  public CoreDatabaseContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<CampaignTemplate>? CampaignTemplates { get; set; }
  public DbSet<Project>? Projects { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(CoreDatabaseContext<TUser>)));
    base.OnModelCreating(modelBuilder);
  }
}
