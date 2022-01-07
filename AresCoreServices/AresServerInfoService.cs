using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ares.Core.Messages;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AresCoreServices;

public class AresServerInfoService : AresServerInfo.AresServerInfoBase
{
  [AllowAnonymous]
  public override Task<ServerInfoResponse> GetServerInfo(Empty request, ServerCallContext context)
  {
    var serverInfo = new ServerInfoResponse
    {
      ServerName = Assembly.GetEntryAssembly()?.GetName().Name,
      Version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
    };

    return Task.FromResult(serverInfo);
  }

  [AuthorizeRoles(AresUserType.AresUser)]
  public override Task GetServerStatusStream(Empty request, IServerStreamWriter<ServerStatusResponse> responseStream, ServerCallContext context)
  {
    var observable = ServerStatusHelper.ServerStatusSubject.AsObservable();
    observable.Subscribe(response => responseStream.WriteAsync(response), () => {}, context.CancellationToken);
    return Task.CompletedTask;
  }
}
