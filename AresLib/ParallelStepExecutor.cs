using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class ParallelStepExecutor : StepExecutor
  {
    public override Task Execute()
    {
      var startTime = DateTime.Now;
      foreach (var command in Commands)
      {
        command.Start();
      }
      return Task.WhenAll(Commands);
    }
  }
}
