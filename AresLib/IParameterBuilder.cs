using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface IParameterBuilder : IBuilder<Parameter>
{
  ParameterMetadata Metadata { get; }
  ParameterValue Value { get; set; }
  bool Planned { get; set; }
}
