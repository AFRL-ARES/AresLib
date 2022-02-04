using System;
using System.Collections.ObjectModel;
using System.Linq;
using Ares.Messaging;
using DynamicData;

namespace Ares.AutomationBuilding;

public class StepTemplateBuilder : TemplateBuilder<StepTemplate>, IStepTemplateBuilder
{
  public StepTemplateBuilder(string name, bool isParallel) : base(name)
  {
    IsParallel = isParallel;

    CommandTemplateBuildersSource
      .Connect()
      .Bind(out var commandTemplateBuilders)
      .Subscribe();

    CommandTemplateBuilders = commandTemplateBuilders;
  }

  private ISourceList<ICommandTemplateBuilder> CommandTemplateBuildersSource { get; }
    = new SourceList<ICommandTemplateBuilder>();

  public bool IsParallel { get; }

  public override StepTemplate Build()
  {
    var commandTemplates = CommandTemplateBuilders.Select
      (
       (commandTemplateBuilder, index) =>
       {
         var commandTemplate = commandTemplateBuilder.Build();
         commandTemplate.Index = index;
         return commandTemplate;
       }
      );

    var stepTemplate = new StepTemplate();
    stepTemplate.CommandTemplates.AddRange(commandTemplates);
    stepTemplate.Name = Name;
    stepTemplate.IsParallel = IsParallel;
    return stepTemplate;
  }

  public ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata)
  {
    var commandTemplateBuilder = new CommandTemplateBuilder(commandMetadata);
    CommandTemplateBuildersSource.Add(commandTemplateBuilder);
    return commandTemplateBuilder;
  }

  public void RemoveCommandTemplateBuilder(ICommandTemplateBuilder templateBuilder)
  {
    CommandTemplateBuildersSource.Remove(templateBuilder);
  }

  public ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
}