using System;
using System.Linq;
using Ares.Messaging;

namespace Ares.AutomationBuilding;

public class ParameterBuilder : IParameterBuilder
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

    ThrowIfInvalid(commandParameter);

    return commandParameter;
  }

  private void ThrowIfInvalid(Parameter parameter)
  {
    if (parameter.Planned)
    {
      return;
    }

    if (!parameter.Metadata.Constraints.Any())
    {
      return;
    }

    // TODO: There could be more than 1 "limits" (imagine discrete ranges of limits) that are not handled yet.
    var limits = parameter.Metadata.Constraints[0];

    if (parameter.Value >= limits.Minimum
        && parameter.Value <= limits.Maximum)
    {
      return;
    }

    throw new Exception($"{parameter.Metadata.Name} with value {parameter.Value} is invalid with constraints {parameter.Metadata.Constraints.Select(l => $"[{l.Minimum}, {l.Maximum}] ").Aggregate((left, right) => $"{left}, {right}")}");
  }

  public ParameterMetadata Metadata { get; }

  public double Value { get; set; }
}