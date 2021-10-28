using System;
using System.Linq;
using Ares.Core;
using AresLibTests.DummyModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitsNet;

namespace AresLibTests.Tests
{
  [TestClass]
  public class CampaignExecution
  {

    [TestMethod]
    public void NoExceptions()
    {
      Console.WriteLine("Yay, a test");
      var testLabManager = new TestLaboratoryManager();
      var testCampaignBuilder = testLabManager.GenerateCampaignBuilder("TestCampaign");
      // Add new experiment builder
      var experimentTemplateBuilder = testCampaignBuilder.AddExperimentTemplateBuilder();
      var step1 = experimentTemplateBuilder.AddStepTemplateBuilder("First", false);
      var step2 = experimentTemplateBuilder.AddStepTemplateBuilder("Second", true);
      var step3 = experimentTemplateBuilder.AddStepTemplateBuilder("Third", false);

      var waitCommandMetadata =
        testLabManager
          .Lab
          .DeviceInterpreters
          .First
            (
             interpeter =>
               interpeter
                 .CommandsToIndexedMetadatas()
                 .Any(commandMetadata => commandMetadata.Name.Equals($"{TestCoreDeviceCommand.Wait}"))
            )
          .CommandsToIndexedMetadatas()
          .First(commandMetadata => commandMetadata.Name.Equals($"{TestCoreDeviceCommand.Wait}"));

      var waitCommandDurationParameterMetadata =
        waitCommandMetadata
          .ParameterMetadatas
          .First(parameterMetadata => parameterMetadata.Name.Equals(Duration.Info.Name));

      // Step 1 stuff. Sigh. So much code.
      var commandBuilder11 = step1.AddCommandTemplateBuilder(waitCommandMetadata);
//      var commandBuilder12 = step1.AddCommandTemplateBuilder(waitCommandMetadata);
      var commandBuilder12 = step1.AddCommandTemplateBuilder(new CommandMetadata(waitCommandMetadata));
      var parameterBuilder111 = commandBuilder11.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );
      var parameterBuilder121 = commandBuilder12.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );

      parameterBuilder111.Value = 2;
      parameterBuilder121.Value = 1;

      // Step 2 stuff. Sigh. So much code.
      var commandBuilder21 = step2.AddCommandTemplateBuilder(waitCommandMetadata);
      var commandBuilder22 = step2.AddCommandTemplateBuilder(waitCommandMetadata);
      var commandBuilder23 = step2.AddCommandTemplateBuilder(waitCommandMetadata);
      var parameterBuilder211 = commandBuilder21.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );
      var parameterBuilder221 = commandBuilder22.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );
      var parameterBuilder231 = commandBuilder23.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );

      parameterBuilder211.Value = 3;
      parameterBuilder221.Value = 2;
      parameterBuilder231.Value = 3;

      // Step 3 stuff. Sigh. So much code.
      var commandBuilder31 = step3.AddCommandTemplateBuilder(waitCommandMetadata);
      var commandBuilder32 = step3.AddCommandTemplateBuilder(waitCommandMetadata);
      var parameterBuilder311 = commandBuilder31.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );
      var parameterBuilder321 = commandBuilder32.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );

      parameterBuilder311.Value = 3;
      parameterBuilder321.Value = 1;

      var testCampaignTemplate = testCampaignBuilder.Build();





      // Database stuff for learning purposes
      var dbContext = new CoreDatabaseContext();
      var deleted = dbContext.Database.EnsureDeleted();
      if (deleted)
      {
        Console.WriteLine("Deleted database");
      }
      var exists = dbContext.Database.EnsureCreated();
      if (exists)
      {
        Console.WriteLine("Database created");
      }
      dbContext.Add(testCampaignTemplate);
      dbContext.SaveChanges();


      var freshDbConnection = new CoreDatabaseContext();
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
      // TODO: force template in RunCampaign to be database lookup
      var cmpr = dbCampaignTemplate.ToString();
      var src = testCampaignTemplate.ToString();
      for (int i = 0; i < src.Length; i++)
      {
        if (src[i] != cmpr[i])
        {
          Console.Write("ú");
          continue;
        }

        Console.Write(" ");
      }
      Console.WriteLine();
      Console.WriteLine(src);
      Console.WriteLine(cmpr);
      Assert.AreEqual(testCampaignTemplate, dbCampaignTemplate);
      testLabManager.RunCampaign(dbCampaignTemplate);
      Console.WriteLine("Did test things");
    }

  }
}
