using System;
using System.Threading.Tasks;

namespace AresLib.Executors
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
