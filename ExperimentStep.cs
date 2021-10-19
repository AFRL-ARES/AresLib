using System;

namespace AresLib
{
  internal class ExperimentStep
  {
    public string Name { get; init; }
    public bool IsParallel { get; init; }
    public Guid Id { get; } = Guid.NewGuid();
    public AresCommand[] Commands { get; init; }
  }
}
