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
  internal class CampaignTemplateBuilder : TemplateBuilder<CampaignTemplate>, ICampaignTemplateBuilder
  {
    public CampaignTemplateBuilder(string name) : base(name)
    {
      ExperimentTemplateBuildersSource
        .Connect()
        .Bind(out var experimentTemplateBuilders)
        .Subscribe();

      ExperimentTemplateBuilders = experimentTemplateBuilders;
    }

    public override CampaignTemplate Build()
    {
      var experimentTemplates =
        ExperimentTemplateBuilders.Select(experimentTemplateBuilder => experimentTemplateBuilder.Build());

      var campaignTemplate = new CampaignTemplate();
      campaignTemplate.ExperimentTemplates.AddRange(experimentTemplates);
      campaignTemplate.Name = Name;

      return campaignTemplate;
    }

    public IExperimentTemplateBuilder AddExperimentTemplateBuilder()
    {
      var experimentTemplateBuilder = new ExperimentTemplateBuilder($"{Name}_Experiment_{ExperimentTemplateBuilders.Count + 1}");
      ExperimentTemplateBuildersSource.AddOrUpdate(experimentTemplateBuilder);
      return experimentTemplateBuilder;
    }

    public void RemoveExperimentTemplateBuilder(string experimentName)
    {
      ExperimentTemplateBuildersSource.Remove(experimentName);
    }

    private ISourceCache<IExperimentTemplateBuilder, string> ExperimentTemplateBuildersSource { get; }
      = new SourceCache<IExperimentTemplateBuilder, string>(experimentTemplateBuilder => experimentTemplateBuilder.Name);

    public ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
  }
}
