using Ares.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Ares.Core.Analyzing;

public class AnalyzerManager : IAnalyzerManager
{
  private readonly IList<IAnalyzer> _analyzerStore = new List<IAnalyzer>();
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;
  readonly AnalysisRepo _analyses;

  public AnalyzerManager(AnalysisRepo analyses, IDbContextFactory<CoreDatabaseContext> dbContextFactory)
  {
    _analyses = analyses;
    _dbContextFactory = dbContextFactory;
    var manualAnalyzer = new NoneAnalyzer();
    _ = RegisterAnalyzer(manualAnalyzer);
    _ = InitializeDbAnalyzers();
  }

  public async Task InitializeDbAnalyzers()
  {
    using var context = await _dbContextFactory.CreateDbContextAsync();
    var availableAnalyzers = context.Analyzers;

    foreach(var info in availableAnalyzers)
    {
      var analyzer = new AresAnalyzer.AresAnalyzer(info.Name, new Uri(info.Address))
      {
        UniqueId = info.UniqueId
      };

      await RegisterAnalyzer(analyzer);
    }
  }

  public void StoreAnalysis(Analysis analysis)
  {
    _analyses.Add(analysis);
  }

  public void ClearAnalyses()
  {
    _analyses.Clear();
  }

  public T GetAnalyzer<T>(Version version) where T : IAnalyzer
  {
    var typedAnalyzers = _analyzerStore.OfType<T>().ToArray();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {typeof(T).Name} in the registry.");

    var analyzer = typedAnalyzers.FirstOrDefault(p => p.Version == version);
    if(analyzer is null)
      throw new KeyNotFoundException($"Unable to find analyzer {typeof(T).Name} with version {version} in the registry.");

    return analyzer;
  }

  public IAnalyzer? GetAnalyzerByName(string name) => _analyzerStore.FirstOrDefault(analyzer => analyzer.Name == name);

  public T GetAnalyzer<T>(string name, Version version) where T : IAnalyzer
  {
    var typedAnalyzers = _analyzerStore.OfType<T>().ToArray();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {typeof(T).Name} in the registry.");

    var versionedAnalyzers = typedAnalyzers.Where(p => p.Name == name);
    if(versionedAnalyzers is null)
      throw new KeyNotFoundException($"Unable to find analyzer of type {typeof(T).Name} named {name} in the registry.");

    var analyzer = versionedAnalyzers.FirstOrDefault(p => p.Version == version);
    if(analyzer is null)
      throw new KeyNotFoundException($"Unable to find analyzer of type {typeof(T).Name} named {name} with version {version} in the registry.");

    return analyzer;
  }

  public IAnalyzer GetAnalyzerByType(string type)
  {
    var typedAnalyzers = _analyzerStore.Where(p => p.GetType().Name == type).ToList();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {type} in the registry.");

    return typedAnalyzers.OrderByDescending(analyzer => analyzer.Version).First();
  }

  public IAnalyzer GetAnalyzer(string type, Version version)
  {
    var typedAnalyzers = _analyzerStore.Where(p => p.GetType().Name == type).ToArray();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {type} in the registry.");

    var analyzer = typedAnalyzers.FirstOrDefault(p => p.Version == version);
    if(analyzer is null)
      throw new KeyNotFoundException($"Unable to find analyzer {type} with version {version} in the registry.");

    return analyzer;
  }

  public IAnalyzer GetAnalyzer(string type, string name, Version version)
  {
    var typedAnalyzers = _analyzerStore.Where(p => p.GetType().Name == type).ToArray();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {type} in the registry.");

    var versionedAnalyzers = typedAnalyzers.Where(p => p.Name == name);
    if(versionedAnalyzers is null)
      throw new KeyNotFoundException($"Unable to find analyzer of type {type} named {name} in the registry.");

    var analyzer = versionedAnalyzers.FirstOrDefault(p => p.Version == version);
    if(analyzer is null)
      throw new KeyNotFoundException($"Unable to find analyzer of type {type} named {name} with version {version} in the registry.");

    return analyzer;
  }

  public IAnalyzer GetAnalyzer(string type, string name)
  {
    var typedAnalyzers = _analyzerStore.Where(p => p.GetType().Name == type).ToArray();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {type} in the registry.");

    var versionedAnalyzers = typedAnalyzers.Where(p => p.Name == name).ToList();
    if(versionedAnalyzers is null)
      throw new KeyNotFoundException($"Unable to find analyzer of type {type} named {name} in the registry.");


    return versionedAnalyzers.OrderByDescending(analyzer => analyzer.Version).First();
  }

  public IAnalyzer GetAnalyzer(AnalyzerInfo info)
  {
    if(!string.IsNullOrEmpty(info.Type) && string.IsNullOrEmpty(info.Name) && string.IsNullOrEmpty(info.Version))
      return GetAnalyzerByType(info.Type);

    if(!string.IsNullOrEmpty(info.Type) && string.IsNullOrEmpty(info.Name) && !string.IsNullOrEmpty(info.Version))
      return GetAnalyzer(info.Type, Version.Parse(info.Version));

    if(!string.IsNullOrEmpty(info.Type) && !string.IsNullOrEmpty(info.Name) && !string.IsNullOrEmpty(info.Version))
      return GetAnalyzer(info.Type, info.Name, Version.Parse(info.Version));

    if(!string.IsNullOrEmpty(info.Type) && !string.IsNullOrEmpty(info.Name) && string.IsNullOrEmpty(info.Version))
      return GetAnalyzer(info.Type, info.Name);

    throw new KeyNotFoundException($"Unable to find an analyzer with the description: {info}");
  }

  public T GetAnalyzer<T>() where T : IAnalyzer
  {
    var typedAnalyzers = _analyzerStore.OfType<T>().ToList();
    if(!typedAnalyzers.Any())
      throw new KeyNotFoundException($"Unable to find any analyzers of type {typeof(T).Name} in the registry.");

    return typedAnalyzers.OrderByDescending(analyzer => analyzer.Version).First();
  }

  public Task RegisterAnalyzer(IAnalyzer analyzer)
  {
    var analyzerExists = _analyzerStore.Any(p => p == analyzer || (p.Name == analyzer.Name && p.Version == analyzer.Version && analyzer.GetType() == p.GetType()));
    if(analyzerExists)
      throw new InvalidOperationException($"Analyzer {analyzer.Name}{analyzer.Version} of type {analyzer.GetType().Name} already registered");

    _analyzerStore.Add(analyzer);
    return Task.CompletedTask;
  }

  public Task UnregisterAnalyzer(IAnalyzer analyzer)
  {
    var analyzerExists = _analyzerStore.Any(p => p == analyzer || (p.Name == analyzer.Name && p.Version == analyzer.Version && analyzer.GetType() == p.GetType()));
    if(!analyzerExists)
      return Task.CompletedTask;

    _analyzerStore.Remove(analyzer);
    return Task.CompletedTask;
  }

  public IEnumerable<IAnalyzer> AvailableAnalyzers => new ReadOnlyCollection<IAnalyzer>(_analyzerStore);
}
