using Ares.Messaging;
using Ares.Tools;

namespace Ares.Core.Execution.Executors;
internal static class ResultGenerator
{
  public static AresStruct GenerateExperimentResult(IEnumerable<StepExecutionSummary> steps, IEnumerable<StepTemplate> stepTemplates)
  {
    var commands = steps.SelectMany(step => step.CommandSummaries);
    var deviceResults = commands
      .Where(cmd => cmd.Result is not null && cmd.Result.Success)
      .Select(cmd => cmd.Result.Result)
      .OfType<AresStruct>();

    var deviceResultStruct = deviceResults.Aggregate((total, next) => total.AppendStruct(next));

    var outputMaps = stepTemplates.SelectMany(st => st.CommandTemplates).Select(ct => ct.UserOutputKeyMap);
    var flattenedOutputMaps = outputMaps
      .SelectMany(map => map)
      .GroupBy(pair => pair.Key) // merge duplicates
      .ToDictionary(group => group.Key, group => group.Last().Value);

    var experimentResultStruct = new AresStruct();
    foreach(var field in deviceResultStruct.Fields)
    {
      var expOutputKey = flattenedOutputMaps[field.Key];
      experimentResultStruct.AddValue(expOutputKey, field.Value);
    }

    return experimentResultStruct;
  }
}
