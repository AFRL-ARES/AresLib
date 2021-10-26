using System;
using System.Linq;
using System.Windows.Input;
using Ares.Core;
using AresLib;
using AresLib.Builders;
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
                 .CommandsToMetadatas()
                 .Any(commandMetadata => commandMetadata.Name.Equals($"{TestCoreDeviceCommand.Wait}"))
            )
          .CommandsToMetadatas()
          .First(commandMetadata => commandMetadata.Name.Equals($"{TestCoreDeviceCommand.Wait}"));

      var waitCommandDurationParameterMetadata = 
        waitCommandMetadata
          .ParameterMetadatas
          .First(parameterMetadata => parameterMetadata.Name.Equals(Duration.Info.Name));

      // Step 1 stuff. Sigh. So much code.
      var commandBuilder11 = step1.AddCommandTemplateBuilder(waitCommandMetadata);
      var commandBuilder12 = step1.AddCommandTemplateBuilder(waitCommandMetadata);
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

      parameterBuilder221.Value = 3;
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
      // TODO: push template to database
      // TODO: replace template in RunCampaign with template lookup to database (force execution to be from database)
      testLabManager.RunCampaign(testCampaignTemplate);
    }

  }
}
