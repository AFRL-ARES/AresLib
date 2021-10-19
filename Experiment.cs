using System;

namespace AresLib
{
  internal class Experiment
  {
    public Guid Id { get; } = Guid.NewGuid();
    public ExperimentStep[] Steps { get; init; }
  }
}
