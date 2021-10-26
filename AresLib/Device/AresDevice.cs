namespace AresLib.Device
{
  public abstract class AresDevice : IAresDevice
  {
    protected AresDevice(string name)
    {
      Name = name;
    }
    public string Name { get; }
  }
}
