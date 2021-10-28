using Ares.Core;

namespace AresLib.Builders
{
  public interface IParameterBuilder : IBuilder<Parameter>
  {
    ParameterMetadata Metadata { get; }
    double Value { get; set; }
  }
}
