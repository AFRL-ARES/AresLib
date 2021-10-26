using Ares.Core;

namespace AresLib.Builders
{
  internal interface ICommandParameterBuilder : IBuilder<CommandParameter>
  {
    CommandParameterMetadata Metadata { get; init; }
    double Value { get; init; }
  }
}
