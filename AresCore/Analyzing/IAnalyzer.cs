using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Analyzing;

public interface IAnalyzer
{
  /// <summary>
  /// Optional name for the analyzer (can be useful when multiple analyzers of same type and version have to be used)
  /// </summary>
  string Name { get; }

  /// <summary>
  /// Version of the analyzer
  /// </summary>
  Version Version { get; }

  /// <summary>
  /// Current state (<see cref="AnalyzerState" />) of the analyzer which essentially indicated
  /// whether or not this analyzer is currently available for analyzing
  /// </summary>
  IObservable<AnalyzerState> AnalyzerState { get; }

  /// <summary>
  /// Returns the values for the given parameter metadata
  /// </summary>
  /// <param name="experiment">The experiment to analyze</param>
  /// <returns>Collection of plan <see cref="PlanResult" /> which has the metadata and the value</returns>
  Task<Analysis> Analyze(CompletedExperiment experiment);
}
