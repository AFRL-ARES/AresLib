using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ares.Core.Grpc;

public static class ServiceMapper
{
  public static void MapCoreAresServices(this IEndpointRouteBuilder routeBuilder)
  {
    routeBuilder.MapGrpcService<DevicesService>();
    routeBuilder.MapGrpcService<AresServerInfoService>();
  }
}
