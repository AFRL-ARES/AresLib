using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ares.Core.Execution.Executors
{
  public class ExperimentExecutorResult
  {
    public ExperimentExecutor? ExperimentExecutor { get; set; }

    public string? ErrorString { get; set; }
  }
}
