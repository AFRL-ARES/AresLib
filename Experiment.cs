using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class Experiment
  {
    public Guid Id { get; }
    public ExperimentStep[] Steps { get; }
    public byte[] OutputData { get; }
    public string OutputFormat { get; }
  }
}
