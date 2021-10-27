using Ares.Core;

namespace AresLib.Builders
{
  public interface ICommandTemplateBuilder : ITemplateBuilder<CommandTemplate>
  {
    ICommandParameterBuilder[] ParameterBuilders { get; }
  }
}
