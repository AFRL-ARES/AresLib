using System;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core.Analyzing;
using Ares.Core.Exceptions;
using Ares.Messaging;
using Ares.Messaging.Analyzing;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

public class AnalyzerService(IAnalyzerRepo analyzerRepo) : AresAnalyzerManagementService.AresAnalyzerManagementServiceBase
{
  private IAnalyzerRepo _analyzerRepo = analyzerRepo;

  public override async Task<GetAllAnalyzersResponse> GetAllAnalyzers(Empty request, ServerCallContext context)
  {
    var response = new GetAllAnalyzersResponse();
    var availableAnalyzers = _analyzerRepo.AvailableAnalyzers;
    var infos = await Task.WhenAll(availableAnalyzers.Select(GetInfo));
    response.Analyzers.AddRange(infos);

    return response;
  }

  private async Task<AnalyzerInfo> GetInfo(IAnalyzer analyzer)
  {
    var info = new AnalyzerInfo
    {
      Name = analyzer.Name,
      Version = analyzer.Version,
      UniqueId = analyzer.UniqueId,
      Capabilities = await analyzer.GetCapabilities(),
      Location = analyzer is RemoteAnalyzer ? AnalyzerLocation.Remote : AnalyzerLocation.Internal
    };

    return info;
  }

  public override Task<AddRemoteAnalyzerResponse> AddRemoteAnalyzer(
    AddRemoteAnalyzerRequest request,
    ServerCallContext context)
  {
    var response = new AddRemoteAnalyzerResponse();
    var uriValid = Uri.TryCreate(request.Url, UriKind.Absolute, out var uri);
    if(!uriValid || uri is null)
    {
      response.Success = false;
      response.ErrorMessage = $"Failed to add analyzer due to the url {request.Url} being invalid";
      return Task.FromResult(response);
    }

    var analyzer = new RemoteAnalyzer(request.Name, uri);
    _analyzerRepo.RegisterAnalyzer(analyzer);

    response.Success = true;
    response.AnalyzerId = analyzer.UniqueId;

    return Task.FromResult(response);
  }

  public override Task<UpdateRemoteAnalyzerResponse> UpdateRemoteAnalyzer(
    UpdateRemoteAnalyzerRequest request,
    ServerCallContext context)
  {
    try
    {
      var analyzer = _analyzerRepo.GetAnalyzerById(request.AnalyzerId);
      var response = UpdateAnalyzer(analyzer, request);
      response.Success = true;
      return Task.FromResult(response);
    }
    catch(ItemNotFoundException e)
    {
      var response = new UpdateRemoteAnalyzerResponse();
      response.Success = false;
      response.ErrorMessage = e.Message;
      return Task.FromResult(response);
    }
  }

  private static UpdateRemoteAnalyzerResponse UpdateAnalyzer(IAnalyzer analyzer, UpdateRemoteAnalyzerRequest request)
  {
    var response = new UpdateRemoteAnalyzerResponse();
    response.Success = true;
    if(!string.IsNullOrEmpty(request.Name))
    {
      analyzer.Name = request.Name;
    }

    if(!string.IsNullOrEmpty(request.Url))
    {
      var uriValid = Uri.TryCreate(request.Url, UriKind.Absolute, out var uri);
      if(!uriValid || uri is null)
      {
        response.Success = false;
        response.ErrorMessage = $"Failed to update analyzer URL {request.Url} due to it being invalid";
      }
    }

    return response;
  }

  public override Task<Empty> RemoveRemoteAnalyzer(RemoveRemoteAnalyzerRequest request, ServerCallContext context)
  {
    _analyzerRepo.UnregisterAnalyzer(request.AnalyzerId);

    return Task.FromResult(new Empty());
  }

  public override Task<AnalyzerStateResponse> GetState(AnalyzerStateRequest request, ServerCallContext context)
  {
    var response = new AnalyzerStateResponse();
    var analyzer = _analyzerRepo.GetAnalyzerById(request.AnalyzerId);

    response.State = analyzer.AnalyzerState;

    return Task.FromResult(response);
  }

  public override async Task<AnalyzerInfoResponse> GetInfo(AnalyzerInfoRequest request, ServerCallContext context)
  {
    var analyzer = _analyzerRepo.GetAnalyzerById(request.AnalyzerId);
    var info = await GetInfo(analyzer);
    var response = new AnalyzerInfoResponse { Info = info };

    return response;
  }
}