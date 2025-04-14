using Ares.Core.Analyzing;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Ares.Core.Grpc.Services;
public class AnalyzerService : AresAnalyzerService.AresAnalyzerServiceBase
{
  private IAnalyzerManager _analyzerManager;

  public AnalyzerService(IAnalyzerManager analyzerManager)
  {
    _analyzerManager = analyzerManager;
  }

  public override Task<GetAvailableAnalyzersResponse> GetAvailableAnalyzers(Empty request, ServerCallContext context)
  {
    var response = new GetAvailableAnalyzersResponse();
    var availableAnalyzers = _analyzerManager.AvailableAnalyzers;

    foreach(var analyzer in availableAnalyzers)
    {
      var protoAnalzyer = new GenericAnalyzer();
      protoAnalzyer.Name = analyzer.Name;
      //protoAnalzyer.Address = analyzer.Address;
      response.Analyzers.Add(protoAnalzyer);
    }

    return Task.FromResult(response);
  }

  public override Task<Empty> AddAnalyzer(GenericAnalyzer request, ServerCallContext context)
  {
    var uri = new Uri(request.Address);

    var analyzer = new AresAnalyzer.AresAnalyzer(request.Name, uri);
    analyzer.Init();

    _analyzerManager.RegisterAnalyzer(analyzer);
    return Task.FromResult(new Empty());
  }

  public override Task<Empty> RemoveAnalyzer(RemoveAnalyzerRequest request, ServerCallContext context)
  {
    var analyzer = _analyzerManager.GetAnalyzerByName(request.Name);

    if(analyzer is null)
      return Task.FromResult(new Empty());

    _analyzerManager.UnregisterAnalyzer(analyzer);
    return Task.FromResult(new Empty());
  }
}
