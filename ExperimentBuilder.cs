using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AresLib
{
  internal class ExperimentBuilder : IBuilder<Experiment>
  {

    // TODO: Pick up here
    public IList<StepBuilder> StepBuilders { get; } = new List<StepBuilder>();

    public Experiment Build()
    {
      return new Experiment
      {
        Steps = StepBuilders.Select(builder => builder.Build()).ToArray()
      };
    }
  }
}
