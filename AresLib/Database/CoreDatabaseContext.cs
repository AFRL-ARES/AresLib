using System.Reflection;
using Ares.Core.Messages;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace AresLib.Database
{
  public class CoreDatabaseContext : DbContext
  {

    public DbSet<CampaignTemplate> CampaignTemplates { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var npgSqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
      {
        Username = "postgres",
        Password = "a",
        Host = "localhost",
        Database = "TestingDerpDeleteMePlzAndQuitClunkingUpPostgres"
      };
      optionsBuilder.UseNpgsql(npgSqlConnectionStringBuilder.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(CoreDatabaseContext)));
    }
  }
}
