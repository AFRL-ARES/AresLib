using Ares.Core;

namespace AresLib.Builders
{
  public interface ICommandParameterBuilder : IBuilder<Parameter>
  {
    ParameterMetadata Metadata { get; }
    double Value { get; set; }
  }
}
