﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ares.Messaging;
using DynamicData;

namespace Ares.AutomationBuilding;

public class ExperimentTemplateBuilder : TemplateBuilder<ExperimentTemplate>, IExperimentTemplateBuilder
{
  public ExperimentTemplateBuilder(string name) : base(name)
  {
    StepTemplateBuildersSource
      .Connect()
      .Bind(out var stepTemplateBuilders)
      .Subscribe();

    StepTemplateBuilders = stepTemplateBuilders;
  }

  private ISourceCache<IStepTemplateBuilder, string> StepTemplateBuildersSource { get; }
    = new SourceCache<IStepTemplateBuilder, string>(stepTemplateBuilder => stepTemplateBuilder.Name);


  public override ExperimentTemplate Build()
  {
    var stepTemplates = StepTemplateBuilders.Select
      (
       (stepTemplateBuilder, index) =>
       {
         var stepTemplate = stepTemplateBuilder.Build();
         stepTemplate.Index = index;
         return stepTemplate;
       }
      );

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

  public ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
}