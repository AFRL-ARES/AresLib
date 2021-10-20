namespace AresLib
{
  public class CommandMetadata
  {
    public string Name { get; }
    public string Description { get; }
    public string DeviceName { get; }
    public ParameterMetadata[] ParameterMetas { get; set; }
  }
}
