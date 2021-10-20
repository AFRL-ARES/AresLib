namespace AresLib
{
  public class ParameterMetadata
  {
    public string Name { get; init; }
    public string Unit { get; init; }
    public double[] Constraints { get; init; } 

    // Note: You might be in the right area if you're coming back here regarding
    // something along the lines of "optional" parameters, or specific indexing of parameters
  }
}
