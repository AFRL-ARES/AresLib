using Ares.Core;

namespace AresLib.Builders
{
  public interface ICommandParameterBuilder : IBuilder<CommandParameter>
  {
    CommandParameterMetadata Metadata { get; }
    double Value { get; set; }
  }
}
