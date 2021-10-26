using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
  {
    ISourceCache<IStepTemplateBuilder, string> StepTemplateBuildersSource { get; }
  }
}
