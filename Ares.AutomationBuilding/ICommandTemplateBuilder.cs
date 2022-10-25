using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface ICommandTemplateBuilder : ITemplateBuilder<CommandTemplate>
{
  IParameterBuilder[] ParameterBuilders { get; }
}
