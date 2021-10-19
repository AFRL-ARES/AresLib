using System;
using System.Collections.Generic;
using System.Linq;

namespace AresLib
{
  internal class StepBuilder : IBuilder<ExperimentStep>
  {
    private IList<CommandBuilder> DeviceCommands { get; } = new List<CommandBuilder>();
    public string Name { get; set; }
    public bool IsParallel { get; set; }
    public ExperimentStep Build()
    {
      return new ExperimentStep
      {
        Name = Name,
        IsParallel = IsParallel,
        Commands = DeviceCommands.Select(builder => builder.Build()).ToArray()
      };
    }
  }
}
