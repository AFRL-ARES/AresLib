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
  internal class StepTemplateBuilder : TemplateBuilder<StepTemplate>, IStepTemplateBuilder
  {

    public StepTemplateBuilder(string name) : base(name)
    {
      CommandTemplateBuildersSource
        .Connect()
        .Bind(out var commandTemplateBuilders)
        .Subscribe();

      CommandTemplateBuilders = commandTemplateBuilders;
    }

    public override StepTemplate Build()
    {
      var commandTemplates = CommandTemplateBuilders.Select(commandTemplateBuilder => commandTemplateBuilder.Build());
      var stepTemplate = new StepTemplate();
      stepTemplate.CommandTemplates.AddRange(commandTemplates);
      stepTemplate.Name = Name;
      return stepTemplate;
    }

    public ICommandTemplateBuilder AddCommandTemplateBuilder(CommandMetadata commandMetadata)
    {
      var commandTemplateBuilder = new CommandTemplateBuilder(commandMetadata);
      CommandTemplateBuildersSource.AddOrUpdate(commandTemplateBuilder);
      return commandTemplateBuilder;
    }

    public void RemoveCommandTemplateBuilder(string templateBuilderName)
    {
      CommandTemplateBuildersSource.Remove(templateBuilderName);
    }

    private ISourceCache<ICommandTemplateBuilder, string> CommandTemplateBuildersSource { get; }
    = new SourceCache<ICommandTemplateBuilder, string>(commandTemplateBuilder => commandTemplateBuilder.Name);
    public ReadOnlyObservableCollection<ICommandTemplateBuilder> CommandTemplateBuilders { get; }
  }
}
