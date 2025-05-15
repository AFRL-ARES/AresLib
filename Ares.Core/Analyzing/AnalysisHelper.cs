using Ares.Messaging;

namespace Ares.Core.Analyzing;

internal class AnalysisHelper
{
  readonly IAnalyzerRepo _analyzerRepo;
  public AnalysisHelper(IAnalyzerRepo analyzerRepo)
  {
    _analyzerRepo = analyzerRepo;
  }

  public async Task<Analysis> Analyze(AnalyzerInfo? analyzerInfo, ExperimentExecutionSummary experimentSummary, CancellationToken cancellationToken)
  {
    var noneAnalyzer = _analyzerRepo.GetAnalyzer<NoneAnalyzer>();
    var analyzer = analyzerInfo is null ? noneAnalyzer : _analyzerRepo
    .GetAnalyzer(analyzerInfo) ?? throw new InvalidOperationException($"Could not find desired Analyzer! {analyzerInfo.Name}");

    var analyzerInputs = ToAnalyzerInputs(experimentSummary.CompletedExperiment.Results);
    var analysis = await analyzer.Analyze(experimentSummary, analyzerInputs, cancellationToken);
    analysis.CompletedExperiment = experimentSummary.CompletedExperiment;
    experimentSummary.CompletedExperiment.AnalysisResult = analysis.Result;
    return analysis;
  }

  private static IEnumerable<AnalyzerInput> ToAnalyzerInputs(IEnumerable<ExperimentResult> experimentResults)
  {
    return experimentResults.Select(er => new AnalyzerInput { Key = er.Key, Data = er.Data });
  }
}
