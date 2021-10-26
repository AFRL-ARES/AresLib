using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib
{
  internal interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
  {
    ISourceCache<IExperimentTemplateBuilder, string> ExperimentTemplateBuildersSource { get; }
  }
}
