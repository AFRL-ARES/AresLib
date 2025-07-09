using System.Collections.ObjectModel;
using Ares.Core.Exceptions;

namespace Ares.Core.Analyzing;

public class AnalyzerRepo : IAnalyzerRepo
{
  private readonly IList<IAnalyzer> _analyzerStore = [];

  public AnalyzerRepo()
  {
    var manualAnalyzer = new NoneAnalyzer();
    RegisterAnalyzer(manualAnalyzer);
  }

  public IAnalyzer GetAnalyzerByName(string name)
  {
    var analyzer = _analyzerStore.FirstOrDefault(analyzer => analyzer.Name == name);
    if(analyzer is null)
    {
      throw new ItemNotFoundException(name, typeof(IAnalyzer));
    }

    return analyzer;
  }

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

  public IAnalyzer GetAnalyzerById(string id)
  {
    var analyzer = _analyzerStore.FirstOrDefault(analyzer => analyzer.UniqueId == id);
    if(analyzer is null)
    {
      throw new ItemNotFoundException(id, typeof(IAnalyzer));
    }

    return analyzer;
  }

  public void UnregisterAnalyzer(string analyzerId)
  {
    var analyzer = _analyzerStore.FirstOrDefault(analyzer => analyzer.UniqueId == analyzerId);
    if(analyzer is null)
    {
      return;
    }

    _analyzerStore.Remove(analyzer);
  }

  public IEnumerable<IAnalyzer> AvailableAnalyzers => new ReadOnlyCollection<IAnalyzer>(_analyzerStore);
}
