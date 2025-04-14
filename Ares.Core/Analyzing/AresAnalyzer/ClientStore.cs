using Ares.Messaging.Analyzing;
using Grpc.Net.Client;

namespace AresAnalyzer;

public static class ClientStore
{
  public static AresAnalyzerGrpc.AresAnalyzerGrpcClient? AresAnalyzingClient { get; private set; }

  public static void CreateClient(Uri address)
  {
    var channel = GrpcChannel.ForAddress(address);
    AresAnalyzingClient = new AresAnalyzerGrpc.AresAnalyzerGrpcClient(channel);
  }
}