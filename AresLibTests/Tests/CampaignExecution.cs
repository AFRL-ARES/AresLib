using System;
using System.Linq;
using System.Text;
using Ares.Core.Messages;
using AresCoreEntityFramework;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AresLibTests.Tests;

[TestClass]
public class CampaignExecution
{

  [TestMethod]
  public void NoExceptions()
  {
    Console.WriteLine("Yay, a test");
    // var testLabManager = new TestLaboratoryManager();
    // var testCampaignTemplate = DemoCampaignProvider.GetTemplate(testLabManager);

    var project = new Project
    {
      Name = "Important growth project",
      Description = "Very important, no shenanigans"
    };

    // project.CampaignTemplates.Add(testCampaignTemplate);
    var result = ByteString.CopyFrom("Somerandomresult", Encoding.ASCII);
    // var completed = new CompletedExperiment
    // {
    //   Format = "someformat",
    //   SerializedData = result,
    //   Template = testCampaignTemplate.ExperimentTemplates.First()
    // };
    //
    // project.CompletedExperiments.Add(completed);


    // Database stuff for learning purposes
    var dbContext = new CoreDatabaseContext<AresTestUser>(new DbContextOptions<CoreDatabaseContext<AresTestUser>>());
    var deleted = dbContext.Database.EnsureDeleted();
    if (deleted)
      Console.WriteLine("Deleted database");

    var exists = dbContext.Database.EnsureCreated();
    if (exists)
      Console.WriteLine("Database created");

    // dbContext.Add(testCampaignTemplate);
    dbContext.SaveChanges();

    dbContext.Add(project);
    dbContext.SaveChanges();

    var dbContext2 = new CoreDatabaseContext<AresTestUser>(new DbContextOptions<CoreDatabaseContext<AresTestUser>>());

    var project2 = dbContext2.Projects.ToArray().First();

    var test = project2.CompletedExperiments.First().SerializedData.ToString(Encoding.ASCII);

    dbContext.Remove(project);
    dbContext.SaveChanges();


    var freshDbConnection = new CoreDatabaseContext<AresTestUser>(new DbContextOptions<CoreDatabaseContext<AresTestUser>>());
    var dbCampaignTemplate = freshDbConnection
      .CampaignTemplates
      .ToArray()
      .Last();


    var beforeDerps = freshDbConnection.Set<CommandMetadata>()
      .ToArray();

    freshDbConnection.Remove(dbCampaignTemplate);
    freshDbConnection.SaveChanges();

    var derps = freshDbConnection.Set<CommandMetadata>()
      .ToArray();

    //      var cmpr = dbCampaignTemplate.ToString();
    //      var src = testCampaignTemplate.ToString();
    //      for (int i = 0; i < src.Length; i++)
    //      {
    //        if (src[i] != cmpr[i])
    //        {
    //          Console.Write("ú");
    //          continue;
    //        }
    //
    //        Console.Write(" ");
    //      }
    //      Console.WriteLine();
    //      Console.WriteLine(src);
    //      Console.WriteLine(cmpr);
    //      Assert.AreEqual(testCampaignTemplate, dbCampaignTemplate);
    // testLabManager.RunCampaign(dbCampaignTemplate);
    Console.WriteLine("Did test things");
  }
}
