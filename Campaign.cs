using System;

namespace AresLib
{
  internal class Campaign
  {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Experiment[] Experiments { get; init; }
  }
}
