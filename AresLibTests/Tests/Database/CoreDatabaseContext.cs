using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLibTests.Tests.Database.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;

namespace AresLibTests.Tests
{
  public class CoreDatabaseContext : DbContext
  {

    public DbSet<CampaignTemplate> CampaignTemplates { get; set; }

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
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(CoreDatabaseContext)));
    }
  }
}
