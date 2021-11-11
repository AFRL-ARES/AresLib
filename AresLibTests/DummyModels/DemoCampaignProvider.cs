using Ares.Core.Messages;
using AresLib;
using System.Linq;
using UnitsNet;

namespace AresLibTests.DummyModels
{
  static class DemoCampaignProvider
  {
    public static CampaignTemplate GetTemplate(ILaboratoryManager labManager)
    {
      var testCampaignBuilder = labManager.GenerateCampaignBuilder("TestCampaign");
      var experimentTemplateBuilder = testCampaignBuilder.AddExperimentTemplateBuilder();
      var step1 = experimentTemplateBuilder.AddStepTemplateBuilder("First", false);
      var step2 = experimentTemplateBuilder.AddStepTemplateBuilder("Second", true);
      var step3 = experimentTemplateBuilder.AddStepTemplateBuilder("Third", false);

      var waitCommandMetadata =
        labManager
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

      return testCampaignBuilder.Build();
    }
  }
}
