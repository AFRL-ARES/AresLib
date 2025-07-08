using System.Collections.ObjectModel;

namespace Ares.Core.Analyzing;

public class AnalyzerRepo : IAnalyzerRepo
{
  private readonly IList<IAnalyzer> _analyzerStore = [];

  public AnalyzerRepo()
  {
    var manualAnalyzer = new NoneAnalyzer();
    RegisterAnalyzer(manualAnalyzer);
  }

  public IAnalyzer? GetAnalyzerByName(string name) => _analyzerStore.FirstOrDefault(analyzer => analyzer.Name == name);

  public void RegisterAnalyzer(IAnalyzer analyzer)
  {
    var analyzerExists = _analyzerStore.Any(p => p == analyzer || (p.Name == analyzer.Name && p.Version == analyzer.Version && analyzer.GetType() == p.GetType()));
    if(analyzerExists)
      throw new InvalidOperationException($"Analyzer {analyzer.Name}{analyzer.Version} of type {analyzer.GetType().Name} already registered");

    _analyzerStore.Add(analyzer);
  }

  public void UnregisterAnalyzer(IAnalyzer analyzer)
  {
    var analyzerExists = _analyzerStore.Any(p => p == analyzer || (p.Name == analyzer.Name && p.Version == analyzer.Version && analyzer.GetType() == p.GetType()));
    if(!analyzerExists)
      return;

    _analyzerStore.Remove(analyzer);
  }

  public IAnalyzer? GetAnalyzerById(string id) => _analyzerStore.FirstOrDefault(analyzer => analyzer.UniqueId == id);

  public IEnumerable<IAnalyzer> AvailableAnalyzers => new ReadOnlyCollection<IAnalyzer>(_analyzerStore);
}
