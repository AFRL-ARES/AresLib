using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Analyzing;

/// <summary>
/// Analyzer that returns a 0 as its analysis result.
/// Used as a default analyzer in case no actual analyzers are present
/// </summary>
internal class NoneAnalyzer : AnalyzerBase
{
  public NoneAnalyzer() : base("NONE", new Version(1, 0))
  {
  }

  public override Task<RequestedAnalysisData[]> GetSupportedInputs()
  {
    return Task.FromResult(Array.Empty<RequestedAnalysisData>());
  }

  protected override Task<Analysis> AnalyzeInputs(ExperimentExecutionSummary summary, IEnumerable<AnalyzerInput> inputs, CancellationToken cancellationToken)
  {
    var analysis = new Analysis
    {
      UniqueId = Guid.NewGuid().ToString(),
      Analyzer = new AnalyzerInfo
      {
        Name = Name,
        UniqueId = Guid.NewGuid().ToString(),
        Version = Version.ToString()
      },
      Result = 0
    };

    return Task.FromResult(analysis);
  }
}
