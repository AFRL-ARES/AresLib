using System.Collections.Generic;
using System.Linq;

namespace AresLib
{
  internal class ExperimentDbDomainBuilder : IDbDomainBuilder<Experiment>
  {

    // TODO: Pick up here
    public IList<StepDbDomainBuilder> StepBuilders { get; } = new List<StepDbDomainBuilder>();

    public Experiment Build()
    {
      return new Experiment
      {
        Steps = StepBuilders.Select(builder => builder.Build()).ToArray()
      };
    }
  }
}
