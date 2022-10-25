namespace Ares.Device.Tests.Device;

public class TestDevice : AresDevice
{

  public TestDevice() : base("Test Device")
  {
  }

  public override bool Activate()
    => true;
}
