using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal class ExperimentTemplateBuilder : TemplateBuilder<ExperimentTemplate>, IExperimentTemplateBuilder
  {
    public ExperimentTemplateBuilder(string name) : base(name)
    {
      StepTemplateBuildersSource
        .Connect()
        .Bind(out var stepTemplateBuilders)
        .Subscribe();

      StepTemplateBuilders = stepTemplateBuilders;
    }


    public override ExperimentTemplate Build()
    {
      var stepTemplates = StepTemplateBuilders.Select(stepTemplateBuilder => stepTemplateBuilder.Build());
      var experimentTemplate = new ExperimentTemplate();
      experimentTemplate.StepTemplates.AddRange(stepTemplates);
      experimentTemplate.Name = Name;
      return experimentTemplate;
    }

    public IStepTemplateBuilder AddStepTemplateBuilder(string stepName, bool isParallel)
    {
      var stepTemplateBuilder = new StepTemplateBuilder(stepName, isParallel);
      StepTemplateBuildersSource.AddOrUpdate(stepTemplateBuilder);
      return stepTemplateBuilder;
    }

    public void RemoveStepTemplateBuilder(string stepName)
    {
      StepTemplateBuildersSource.Remove(stepName);
    }

    private ISourceCache<IStepTemplateBuilder, string> StepTemplateBuildersSource { get; }
    = new SourceCache<IStepTemplateBuilder, string>(stepTemplateBuilder => stepTemplateBuilder.Name);
    public ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
  }
}
