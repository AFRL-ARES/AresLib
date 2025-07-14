using Ares.Core.Analyzing;
using Ares.Core.Execution.Extensions;
using Ares.Messaging;
using Ares.Tools;
using System.Security.Cryptography;

namespace Ares.Core.Validation.Validators;

public static class GoodAnalyzerValidator
{
  public static async Task<ValidationResult> Validate(ExperimentTemplate experimentTemplate, IAnalyzerRepo analyzerRepo)
  {
    if(experimentTemplate.AnalyzerId is null)
      return new ValidationResult(true);

    var analyzer = analyzerRepo.GetAnalyzerById(experimentTemplate.AnalyzerId);
    if(analyzer is null)
      return new ValidationResult(false, $"Unable to find analyzer with id of {experimentTemplate.AnalyzerId}");

    var outputCommands = experimentTemplate.GetAllOutputCommands();

    var analysisParameterSchema = await analyzer.GetParameters();
    var requiredAnalysisInputs = analysisParameterSchema.Fields.Where(input => !input.Value.Optional).ToArray();
    if(!outputCommands.Any())
    {
      if(!requiredAnalysisInputs.Any())
        return new ValidationResult(true);
      else
        return new ValidationResult(false, $"Experiment does not have any output commands set, but has analyzer {analyzer.Name} assigned");
    }

    var inputSchema = new AresDataSchemaSimplified();

    foreach(var map in experimentTemplate.AnalyzerMaps)
    {
      var matchingCommand = outputCommands.FirstOrDefault(cmd => cmd.UserOutputKeyMap.ContainsKey(map.Key));

      if(matchingCommand is null)
        continue;

      var whatever = matchingCommand.UserOutputKeyMap.FirstOrDefault(userMap => userMap.Value == map.Value);
      var whatevertwopointoh = matchingCommand.Metadata.OutputMetadata.DataSchema.Fields.FirstOrDefault(field => field.Key == whatever.Key);
      inputSchema.AddEntry(whatevertwopointoh.Key, whatevertwopointoh.Value);
    }

    var result = await analyzer.ValidateInputs(inputSchema);
    var validationResult = new ValidationResult(result.Success, result.Messages);

    return validationResult;
  }

  public static async Task<ValidationResult> Validate(IEnumerable<ExperimentTemplate> experimentTemplates, IAnalyzerRepo analyzerManager)
  {
    var validationTasks = experimentTemplates.Select(template => Validate(template, analyzerManager)).ToArray();
    var validations = await Task.WhenAll(validationTasks);
    return new ValidationResult(validations);
  }
}
