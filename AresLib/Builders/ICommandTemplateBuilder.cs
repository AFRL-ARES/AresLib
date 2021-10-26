using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal interface ICommandTemplateBuilder : ITemplateBuilder<CommandTemplate>
  {
    ISourceCache<ICommandParameterBuilder, string> CommandParameterBuildersSource { get; }
  }
}
