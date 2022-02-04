using System.Linq;
using Ares.Messaging;

namespace Ares.AutomationBuilding;

public class CommandTemplateBuilder : TemplateBuilder<CommandTemplate>, ICommandTemplateBuilder
{
  public CommandTemplateBuilder(CommandMetadata commandMetadata) : base(commandMetadata.Name)
  {
    Metadata = new CommandMetadata(commandMetadata);
    ParameterBuilders =
      Metadata
        .ParameterMetadatas
        .Select(parameterMetadata => new ParameterBuilder(parameterMetadata))
        .Cast<IParameterBuilder>()
        .ToArray();
  }

  public CommandMetadata Metadata { get; }

  public override CommandTemplate Build()
  {
    var parameters = ParameterBuilders.Select
      (
       (parameterBuilder, index) =>
       {
         var parameter = parameterBuilder.Build();
         parameter.Index = index;
         return parameter;
       }
      );

    var commandTemplate = new CommandTemplate();
    commandTemplate.Arguments.AddRange(parameters);
    commandTemplate.Metadata = Metadata;
    return commandTemplate;
  }


  public IParameterBuilder[] ParameterBuilders { get; }
}