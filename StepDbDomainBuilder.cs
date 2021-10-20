using System.Collections.Generic;
using System.Linq;

namespace AresLib
{
  internal class StepDbDomainBuilder : IDbDomainBuilder<ExperimentStep>
  {
    private IList<DbDomainCommandBuilder> DeviceCommands { get; } = new List<DbDomainCommandBuilder>();
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
