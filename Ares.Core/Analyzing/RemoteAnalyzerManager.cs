using Ares.Core.Notifications;
using Ares.Messaging.Analyzing;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Analyzing;
public class RemoteAnalyzerManager(IDbContextFactory<CoreDatabaseContext> _dbContextFactory, IAnalyzerRepo _analyzerRepo, INotificationHandler _notificationHandler) : IRemoteAnalyzerManager
{
  public async Task CreateAnalyzer(string name, string url)
  {
    var config = new AnalyzerConfig { UniqueId = Guid.NewGuid().ToString(), Name = name, Url = url };
    var analyzer = await LoadAnalyzer(config);
    if(analyzer is null)
      return;

    _analyzerRepo.AddAnalyzer(analyzer);
    var ctx = _dbContextFactory.CreateDbContext();
    ctx.Analyzers.Add(config);

    await ctx.SaveChangesAsync();
  }

  private async Task<IAnalyzer?> LoadAnalyzer(AnalyzerConfig config)
  {
    var uriValid = Uri.TryCreate(config.Url, UriKind.Absolute, out var uri);
    if(!uriValid || uri is null)
    {
      _ = _notificationHandler.HandleNotification(
        "Analyzer Add Error",
        $"Failed to create a remote analyzer {config.Name} because the url {config.Url} is invalid.",
        NotificationSeverityEnum.Danger);
      return null;
    }
    var analyzer = new RemoteAnalyzer(config.Name, uri, config.UniqueId);

    await analyzer.Init();

    return analyzer;
  }

  public async Task LoadAnalyzers()
  {
    var ctx = _dbContextFactory.CreateDbContext();
    var configs = await ctx.Analyzers.ToArrayAsync();
    var analyzers = await Task.WhenAll(configs.Select(LoadAnalyzer));
    var nonNullAnalyzers = analyzers.OfType<IAnalyzer>().ToArray();
    foreach(var analyzer in nonNullAnalyzers)
    {
      _analyzerRepo.AddAnalyzer(analyzer);
    }
  }

  public async Task RemoveAnalyzer(string analyzerId)
  {
    var ctx = _dbContextFactory.CreateDbContext();
    var analyzer = ctx.Analyzers.Where(a => a.UniqueId == analyzerId).FirstOrDefault();
    if(analyzer is null)
    {
      return;
    }

    ctx.Remove(analyzer);
    await ctx.SaveChangesAsync();

    _analyzerRepo.RemoveAnalyzer(analyzerId);
  }

  public async Task UpdateAnalyzer(AnalyzerConfig config)
  {
    var ctx = _dbContextFactory.CreateDbContext();
    var analyzerCfg = ctx.Analyzers.Where(a => a.UniqueId == config.UniqueId).FirstOrDefault();
    if(analyzerCfg is null)
    {
      return;
    }

    analyzerCfg.Name = config.Name;
    analyzerCfg.Url = config.Url;
    await ctx.SaveChangesAsync();

    _analyzerRepo.RemoveAnalyzer(analyzerCfg.UniqueId);
    var analyzer = await LoadAnalyzer(analyzerCfg);
    if(analyzer is null)
    {
      return;
    }
    _analyzerRepo.AddAnalyzer(analyzer);
  }
}
