using Ares.Messaging;
using Ares.Messaging.Analyzing;
using Ares.Messaging.Analyzing.Remote;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace Ares.Core.Analyzing;
public class RemoteAnalyzer : AnalyzerBase
{
  private readonly GrpcChannel _channel;
  private bool _initialized = false;

  public RemoteAnalyzer(string name, Uri address, string id) : base(name, "", "_._._", id)
  {
    _channel = GrpcChannel.ForAddress(address);
    Address = address;
  }

  public RemoteAnalyzer(string name, Uri address) : base(name, "", "_._._")
  {
    _channel = GrpcChannel.ForAddress(address);
    Address = address;
  }

  public Uri Address { get; }

  public override Task<Analysis> Analyze(AresStruct inputs, CancellationToken cancellationToken = default)
  {
    var client = GetClient();
    var analysisRequest = new AnalysisRequest
    {
      Inputs = inputs,
      Settings = new AresStruct()
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

  public override async Task<AnalyzerCapabilities> GetCapabilities(CancellationToken cancellationToken = default)
  {
    var client = GetClient();
    try
    {
      var capabilities = await client.GetAnalyzerCapabilitiesAsync(new Empty(), cancellationToken: cancellationToken);
      return capabilities;
    }
    catch(RpcException)
    {
      return new AnalyzerCapabilities();
    }

  }

  public override async Task<AresDataSchema> GetParameters(CancellationToken cancellationToken)
  {
    var client = GetClient();
    try
    {
      var response = await client.GetAnalysisParametersAsync(new Empty(), cancellationToken: cancellationToken);
      return response.ParameterSchema;
    }
    catch(RpcException)
    {
      return new AresDataSchema();
    }
  }

  public override async Task Init()
  {
    await UpdateInfo();
    await UpdateState();
  }

  internal async Task UpdateInfo()
  {
    var client = GetClient();
    try
    {
      var info = await client.GetInfoAsync(new Empty());
      Type = info.Name;
      Version = info.Version;
      Description = info.Description;
    }
    catch(RpcException)
    {
      Type = "Unknown";
      Version = "Unknown";
      Description = "Failed to retrieve analyzer information";
    }
  }

  internal async Task UpdateState()
  {
    var client = GetClient();
    try
    {
      var state = await client.GetStateAsync(new Empty());
      AnalyzerState = state.State;
      StateMessage = state.StateMessage;
      _initialized = true;
    }
    catch(RpcException e)
    {
      AnalyzerState = AnalyzerState.Inactive;
      StateMessage = $"Failed to connect to analyzer: {e.Message}";
    }
  }

  private AresRemoteAnalyzerService.AresRemoteAnalyzerServiceClient GetClient()
  {
    return new AresRemoteAnalyzerService.AresRemoteAnalyzerServiceClient(_channel);
  }
}
