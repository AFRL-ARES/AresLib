using Ares.Core.Analyzing;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services;
public class AnalyzerService : AresAnalyzerService.AresAnalyzerServiceBase
{
  private IAnalyzerManager _analyzerManager;
  private readonly IDbContextFactory<CoreDatabaseContext> _coreContextFactory;

  public AnalyzerService(IAnalyzerManager analyzerManager, IDbContextFactory<CoreDatabaseContext> coreContextFactory)
  {
    _analyzerManager = analyzerManager;
    _coreContextFactory = coreContextFactory;
  }

  public override Task<GetAvailableAnalyzersResponse> GetAvailableAnalyzers(Empty request, ServerCallContext context)
  {
    var response = new GetAvailableAnalyzersResponse();
    var availableAnalyzers = _analyzerManager.AvailableAnalyzers;

    foreach(var analyzer in availableAnalyzers)
    {
      var protoAnalzyer = new GenericAnalyzer();
      protoAnalzyer.Name = analyzer.Name;
      protoAnalzyer.Address = analyzer.Address;
      response.Analyzers.Add(protoAnalzyer);
    }

    return Task.FromResult(response);
  }

  public override async Task<Empty> UpdateAnalzyer(GenericAnalyzer request, ServerCallContext context)
  {
    var existingAnalyzer = _analyzerManager.GetAnalyzerByName(request.Name);
    await using var dbContext = await _coreContextFactory.CreateDbContextAsync();

    if(existingAnalyzer is null || existingAnalyzer.Name == request.Name && existingAnalyzer.Address == request.Address)
      return new Empty();

    await _analyzerManager.UnregisterAnalyzer(existingAnalyzer);

    var updatedAnalyzer = new AresAnalyzer.AresAnalyzer(request.Name, new Uri(request.Address));
    updatedAnalyzer.Init();
    await _analyzerManager.RegisterAnalyzer(updatedAnalyzer);
    await RemoveAnalyzerFromDb(existingAnalyzer.Name, context);
    await AddAnalyzerToDb(updatedAnalyzer, context);
    return new Empty();
  }

  public override async Task<Empty> AddAnalyzer(GenericAnalyzer request, ServerCallContext context)
  {
    if(_analyzerManager.AvailableAnalyzers.Any(a => a.Name == request.Name))
      return new Empty();

    var uri = new Uri(request.Address);
    var analyzer = new AresAnalyzer.AresAnalyzer(request.Name, uri);
    analyzer.Init();
    await _analyzerManager.RegisterAnalyzer(analyzer);
    await AddAnalyzerToDb(analyzer, context);
    return new Empty();
  }

  public override async Task<Empty> RemoveAnalyzer(RemoveAnalyzerRequest request, ServerCallContext context)
  {
    var analyzer = _analyzerManager.GetAnalyzerByName(request.Name);

    if(analyzer is null)
      return new Empty();

    await _analyzerManager.UnregisterAnalyzer(analyzer);
    await RemoveAnalyzerFromDb(request.Name, context);
    return new Empty();
  }

  private async Task AddAnalyzerToDb(AresAnalyzer.AresAnalyzer analyzer, ServerCallContext context)
  {
    var info = new AnalyzerInfo()
    {
      Name = analyzer.Name,
      Address = analyzer.Address,
      Type = analyzer.GetType().ToString(),
      Version = analyzer.Version.ToString(),
      UniqueId = analyzer.UniqueId
    };

    await using var dbContext = await _coreContextFactory.CreateDbContextAsync();
    await dbContext.Analyzers.AddAsync(info);
    await dbContext.SaveChangesAsync(context.CancellationToken);
  }

  private async Task RemoveAnalyzerFromDb(string name, ServerCallContext context)
  {
    await using var dbContext = await _coreContextFactory.CreateDbContextAsync();
    var oldInfo = await dbContext.Analyzers.FirstOrDefaultAsync(a => a.Name == name);
    if(oldInfo != null)
      dbContext.Analyzers.Remove(oldInfo);
    await dbContext.SaveChangesAsync(context.CancellationToken);
  }
}
