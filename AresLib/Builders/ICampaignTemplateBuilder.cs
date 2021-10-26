using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
  {
    ISourceCache<IExperimentTemplateBuilder, string> ExperimentTemplateBuildersSource { get; }
  }
}
