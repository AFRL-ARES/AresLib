using Ares.Messaging;
using Ares.Messaging.Analyzing;
using Ares.Messaging.Analyzing.Remote;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace Ares.Core.Analyzing;
internal class RemoteAnalyzer(string name, Uri address) : AnalyzerBase(name, "", "_._._")
{
  private readonly GrpcChannel _channel = GrpcChannel.ForAddress(address);

  public override Task<Analysis> Analyze(AresStruct inputs, CancellationToken cancellationToken = default)
  {
    var client = GetClient();
    var analysisRequest = new AnalysisRequest
    {
      Inputs = inputs
    };
    return client.AnalyzeAsync(analysisRequest, cancellationToken: cancellationToken).ResponseAsync;
  }

  public override Task<Analysis> Analyze(AresStruct inputs, AresStruct settings, CancellationToken cancellationToken = default)
  {
    var client = GetClient();
    var analysisRequest = new AnalysisRequest
    {
      Inputs = inputs,
      Settings = settings
    };
    return client.AnalyzeAsync(analysisRequest, cancellationToken: cancellationToken).ResponseAsync;
  }

  public override Task<AnalyzerCapabilities> GetCapabilities(CancellationToken cancellationToken = default)
  {
    var client = GetClient();
    return client.GetAnalyzerCapabilitiesAsync(
      new Empty(),
      cancellationToken: cancellationToken)
      .ResponseAsync;

  }

  public override async Task<AresDataSchema> GetParameters(CancellationToken cancellationToken)
  {
    var client = GetClient();
    var response = await client.GetAnalysisParametersAsync(new Empty(), cancellationToken: cancellationToken);
    return response.ParameterSchema;
  }

  public override async Task Init()
  {
    var client = GetClient();
    var info = await client.GetInfoAsync(new Empty());
    Type = info.Name;
    Version = info.Version;
    Description = info.Description;
  }

  private AresRemoteAnalyzerService.AresRemoteAnalyzerServiceClient GetClient()
  {
    return new AresRemoteAnalyzerService.AresRemoteAnalyzerServiceClient(_channel);
  }
}
