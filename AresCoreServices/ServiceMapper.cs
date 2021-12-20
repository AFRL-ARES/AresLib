using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AresCoreServices
{
  public static class ServiceMapper
  {
    public static void MapCoreAresServices(this IEndpointRouteBuilder routeBuilder)
    {
      routeBuilder.MapGrpcService<DevicesService>();
    }
  }
}
