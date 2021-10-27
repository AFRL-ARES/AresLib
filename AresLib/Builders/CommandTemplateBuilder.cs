using Ares.Core;
using System.Linq;

namespace AresLib.Builders
{
  internal class CommandTemplateBuilder : TemplateBuilder<CommandTemplate>, ICommandTemplateBuilder
  {
    public CommandTemplateBuilder(CommandMetadata commandMetadata) : base(commandMetadata.Name)
    {
      Metadata = commandMetadata;
      ParameterBuilders =
        Metadata
          .ParameterMetadatas
          .Select(parameterMetadata => new CommandParameterBuilder(parameterMetadata))
          .Cast<ICommandParameterBuilder>()
          .ToArray();
    }

    public override CommandTemplate Build()
    {
      var commandParameters = ParameterBuilders.Select(parameterBuilder => parameterBuilder.Build());
      var commandTemplate = new CommandTemplate();
      commandTemplate.Arguments.AddRange(commandParameters);
      commandTemplate.Metadata = Metadata;
      return commandTemplate;
    }


    public ICommandParameterBuilder[] ParameterBuilders { get; }

    public CommandMetadata Metadata { get; }
  }
}
