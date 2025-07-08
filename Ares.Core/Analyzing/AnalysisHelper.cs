using Ares.Messaging;
using Ares.Messaging.Analyzing;

namespace Ares.Core.Analyzing;

internal class AnalysisHelper
{
  readonly IAnalyzerRepo _analyzerRepo;
  public AnalysisHelper(IAnalyzerRepo analyzerRepo)
  {
    _analyzerRepo = analyzerRepo;
  }

  public async Task<Analysis> Analyze(string? analyzerId, ExperimentExecutionSummary experimentSummary, CancellationToken cancellationToken)
  {
    var analyzer = GetAnalyzer(analyzerId);

    var analyzerInputs = ToAnalysisInputs(experimentSummary.CompletedExperiment.Results);
    // TODO: Add support for settings
    var analysis = await analyzer.Analyze(analyzerInputs, cancellationToken);
    experimentSummary.CompletedExperiment.AnalysisResult = analysis.Result;
    return analysis;
  }

  private IAnalyzer GetAnalyzer(string? analyzerId)
  {
    if(analyzerId is null)
    {
      var noneAnalyzer = _analyzerRepo.GetAnalyzerByName("NONE");
      if(noneAnalyzer is null)
      {
        throw new InvalidOperationException(
          "No analyzer provided and the default NONE analyzer was not found.");
      }

      return noneAnalyzer;
    }

    return _analyzerRepo
    .GetAnalyzerById(analyzerId) ?? throw new InvalidOperationException($"Could not find desired analyzer with id {analyzerId}");
  }

  private static AresStruct ToAnalysisInputs(IEnumerable<ExperimentResult> experimentResults)
  {
    var aresStruct = new AresStruct();
    foreach(var result in experimentResults)
    {
      aresStruct.Fields[result.Key] = result.Data;
    }
    ;

    return aresStruct;
  }
}
