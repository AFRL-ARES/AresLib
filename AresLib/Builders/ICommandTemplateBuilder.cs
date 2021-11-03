using Ares.Core.Messages;

namespace AresLib.Builders
{
  public interface ICommandTemplateBuilder : ITemplateBuilder<CommandTemplate>
  {
    IParameterBuilder[] ParameterBuilders { get; }
  }
}
