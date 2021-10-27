using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AresLibTests.Tests
{
  public class CoreDatabaseContext : DbContext
  {

//    public DbSet<CampaignTemplate> CampaignTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var npgSqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();
      npgSqlConnectionStringBuilder.Username = "postgres";
      npgSqlConnectionStringBuilder.Password = "a";
      npgSqlConnectionStringBuilder.Host = "localhost";
      npgSqlConnectionStringBuilder.Database = "TestingDerpDeleteMePlzAndQuitClunkingUpPostgres";
      optionsBuilder.UseNpgsql(npgSqlConnectionStringBuilder.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
//      modelBuilder.Entity<CampaignTemplate>()
    }
  }
}
