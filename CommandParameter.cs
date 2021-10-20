using System;

namespace AresLib
{
  public class CommandParameter
  {
    public ParameterMetadata Metadata { get; init; }
    public double Value { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
  }
}
