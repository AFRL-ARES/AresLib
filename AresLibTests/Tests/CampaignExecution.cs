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
      var step1 = experimentTemplateBuilder.AddStepTemplateBuilder("First Test Step");
      var step2 = experimentTemplateBuilder.AddStepTemplateBuilder("Second Test Step");
      var step3 = experimentTemplateBuilder.AddStepTemplateBuilder("Third Test Step");

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

      var commandBuilder11 = step1.AddCommandTemplateBuilder(waitCommandMetadata);
      var durationParameterBuilder = commandBuilder11.ParameterBuilders.First
        (
         parameterBuilder =>
           parameterBuilder
             .Metadata
             .Name
             .Equals(waitCommandDurationParameterMetadata.Name)
        );

      durationParameterBuilder.Value = 2;


      var testCampaignTemplate = testCampaignBuilder.Build();
      testLabManager.RunCampaign(testCampaignTemplate);
    }

  }
}
