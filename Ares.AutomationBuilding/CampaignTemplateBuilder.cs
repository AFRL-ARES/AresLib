using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ares.Messaging;
using DynamicData;

namespace Ares.AutomationBuilding;

public class CampaignTemplateBuilder : TemplateBuilder<CampaignTemplate>, ICampaignTemplateBuilder
{
  public CampaignTemplateBuilder(string name) : base(name)
  {
    ExperimentTemplateBuildersSource
      .Connect()
      .Bind(out var experimentTemplateBuilders)
      .Subscribe();

    ExperimentTemplateBuilders = experimentTemplateBuilders;
  }

  private ISourceCache<IExperimentTemplateBuilder, string> ExperimentTemplateBuildersSource { get; }
    = new SourceCache<IExperimentTemplateBuilder, string>(experimentTemplateBuilder => experimentTemplateBuilder.Name);

  public override CampaignTemplate Build()
  {
    var experimentTemplates =
      ExperimentTemplateBuilders.Select
        (
         (experimentTemplateBuilder, index) =>
         {
           var experimentTemplate = experimentTemplateBuilder.Build();
           experimentTemplate.Index = index;
           return experimentTemplate;
         }
        );

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

  public ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
}