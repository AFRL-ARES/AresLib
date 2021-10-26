using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib
{
  internal interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
  {
    ISourceCache<IStepTemplateBuilder, string> StepTemplateBuildersSource { get; }
  }
}
