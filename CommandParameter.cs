using System;

namespace AresLib
{
  public class CommandParameter
  {
    public ParameterMetadata MetaData { get; init; }
    public double Value { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
  }
}
