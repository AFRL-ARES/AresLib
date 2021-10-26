using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal interface IStepTemplateBuilder : ITemplateBuilder<StepTemplate>
  {
    ISourceCache<ICommandTemplateBuilder, string> CommandTemplateBuildersSource { get; }
  }
}
