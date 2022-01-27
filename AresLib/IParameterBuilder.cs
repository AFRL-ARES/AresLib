using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface IParameterBuilder : IBuilder<Parameter>
{
  ParameterMetadata Metadata { get; }
  double Value { get; set; }
}
