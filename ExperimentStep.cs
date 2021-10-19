using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class ExperimentStep
  {
    public string Name { get; }
    public bool IsParallel { get; }
    public Guid Id { get; }
    public AresCommand[] Commands { get; }
  }
}
