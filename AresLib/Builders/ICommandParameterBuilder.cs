using Ares.Core;

namespace AresLib.Builders
{
  public interface ICommandParameterBuilder : IBuilder<CommandParameter>
  {
    CommandParameterMetadata Metadata { get; init; }
    double Value { get; init; }
  }
}
