using Ares.Messaging;

namespace Ares.AutomationBuilding;

internal class ParameterBuilder : IParameterBuilder
{
  public ParameterBuilder(ParameterMetadata parameterMetadata)
  {
    Metadata = parameterMetadata;
  }

  public Parameter Build()
  {
    var commandParameter =
      new Parameter
      {
        Metadata = new ParameterMetadata(Metadata),
        Value = (float)Value
      };

    return commandParameter;
  }

  public ParameterMetadata Metadata { get; }

  public double Value { get; set; }
}
