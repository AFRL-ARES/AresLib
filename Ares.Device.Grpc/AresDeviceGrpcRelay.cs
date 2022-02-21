using Grpc.Net.Client;

namespace Ares.Device.Grpc;

public class AresDeviceGrpcRelay : AresDevice
{

  public AresDeviceGrpcRelay(string name, GrpcChannel channel) : base(name)
  {
  }

  public override Task<bool> Activate()
  {
    throw new NotImplementedException();
  }
}
