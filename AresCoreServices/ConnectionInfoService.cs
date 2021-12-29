using System.Reflection;
using System.Threading.Tasks;
using Ares.Core.Messages;
using Grpc.Core;

namespace AresCoreServices;

public class ConnectionInfoService : RpcConnectionInfo.RpcConnectionInfoBase
{
  public override Task<ConnectionInfoResponse> GetConnectionInfo(ConnectionInfoRequest request, ServerCallContext context)
  {
    var connectionInfo = new ConnectionInfoResponse
    {
      ServiceName = Assembly.GetEntryAssembly()?.GetName().Name,
      Version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
    };

    return Task.FromResult(connectionInfo);
  }
}
