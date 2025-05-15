using System;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core.Analyzing;
using Ares.Messaging;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

class AnalysisService : AresAnalysis.AresAnalysisBase
{
  private readonly IAnalyzerRepo _analyzerRepo;

  public AnalysisService(IAnalyzerRepo analyzerRepo)
  {
    _analyzerRepo = analyzerRepo;
  }

  public override Task<GetAllAnalyzersResponse> GetAllAnalyzers(
    GetAllAnalyzersRequest request,
    ServerCallContext context)
  {
    var response = new GetAllAnalyzersResponse();
    var analyzers = _analyzerRepo.AvailableAnalyzers.Select(analyzer => new AnalyzerInfo { Name = analyzer.Name, Type = analyzer.GetType().Name, Version = analyzer.Version.ToString(), UniqueId = Guid.NewGuid().ToString() });
    response.Analyzers.AddRange(analyzers);
    return Task.FromResult(response);
  }

  public override Task<RequestedAnalysisDataResponse> GetRequestedAnalysisData(RequestedAnalysisDataRequest request, ServerCallContext context)
  {
    return base.GetRequestedAnalysisData(request, context);
  }
}
