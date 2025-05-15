using Ares.Core.Analyzing;
using Ares.Messaging;

namespace Ares.Core.Validation.Validators;

public static class GoodAnalyzerValidator
{
  public static async Task<ValidationResult> Validate(ExperimentTemplate experimentTemplate, IAnalyzerRepo analyzerManager)
  {
    if(experimentTemplate.Analyzer is null)
      return new ValidationResult(true);

    var analyzer = analyzerManager.GetAnalyzer(experimentTemplate.Analyzer);
    if(analyzer is null)
      return new ValidationResult(false, $"Unable to find analyzer {experimentTemplate.Analyzer}");

    var allCommands = experimentTemplate
      .StepTemplates
      .SelectMany(template => template.CommandTemplates)
      .ToArray();

    var outputCommandSelections = experimentTemplate.OutputCommands;

    var outputCommands = allCommands
      .Where(template => experimentTemplate.OutputCommands.Any(c => c.CommandId == template.UniqueId));

    var supportedAnalyzerInputs = await analyzer.GetSupportedInputs();
    var requiredAnalyzerInputs = supportedAnalyzerInputs.Where(si => !si.Optional).ToArray();
    if(!outputCommands.Any())
    {
      if(experimentTemplate.Analyzer is null || !requiredAnalyzerInputs.Any())
        return new ValidationResult(true);
      else
        return new ValidationResult(false, $"Experiment does not have any output commands set, but has analyzer {experimentTemplate.Analyzer} assigned");
    }

    var keyOutputCommandMap = outputCommandSelections.Select(
      ocs =>
      {
        var outputCommand = outputCommands.FirstOrDefault(oc => oc.UniqueId == ocs.CommandId);

        return new { ocs.Key, OutputCommand = outputCommand };
      }).ToArray();

    // This checks for an unlikely scenario that a command id might be set on an experiment template
    // indicating a command that isn't actually there. Maybe something happened that when a command
    // got removed, the output command id was not removed from the template or something similarly weird
    var unfulfilledOutputRequests = keyOutputCommandMap.Where(koc => koc.OutputCommand is null);
    if(unfulfilledOutputRequests.Any())
    {
      return new ValidationResult(false, $"The experiment thinks that it has output commands set with keys {string.Join(',', unfulfilledOutputRequests.Select(uor => uor.Key))}, but there aren't actually any commands within the experiment. The eperiment template might be corrupted in this case.");
    }

    var missingRequiredInputs = requiredAnalyzerInputs.Where(rai => !keyOutputCommandMap.Any(koc => koc.Key == rai.Key)).ToArray();

    if(missingRequiredInputs.Any())
    {
      return new ValidationResult(
        false,
        $"Missing required outputs for analyzer: {string.Join(',', missingRequiredInputs.Select(mri => mri.Key))}");
    }

    var validationRequests = keyOutputCommandMap.Select(
      // OutputCommand can no longer be null because of the check for unfulfilledOutputRequests
      // if that check is removed, the null-forgiving operator should also be eliminated
      koc =>
        new AnalyzerInputValidationRequest
        {
          Key = koc.Key,
          TypeName = koc.OutputCommand!.Metadata.OutputMetadata.FullName
        });


    return await analyzer.ValidateInputs(validationRequests);
  }

  public static async Task<ValidationResult> Validate(IEnumerable<ExperimentTemplate> experimentTemplates, IAnalyzerRepo analyzerManager)
  {
    var validationTasks = experimentTemplates.Select(template => Validate(template, analyzerManager)).ToArray();
    var validations = await Task.WhenAll(validationTasks);
    return new ValidationResult(validations);
  }
}
