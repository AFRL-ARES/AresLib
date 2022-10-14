using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Messaging;

namespace Ares.Core.Analyzing;

/// <summary>
/// Analyzer that returns a 0 as its analysis result.
/// Used as a default analyzer in case no actual analyzers are present
/// </summary>
internal class NoneAnalyzer : IAnalyzer
{
  public string Name { get; } = "NONE";
  public Version Version { get; } = new(1, 0);

  public Task<Analysis> Analyze(CompletedExperiment experiment, CancellationToken _)
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
      CompletedExperiment = experiment,
      Result = 0
    };

    return Task.FromResult(analysis);
  }

  public IObservable<AnalyzerState> AnalyzerState { get; } = new BehaviorSubject<AnalyzerState>(Analyzing.AnalyzerState.Connected).AsObservable();
}
