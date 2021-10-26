using System;
using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal class ParallelStepExecutor : StepExecutor
  {
    public ParallelStepExecutor(string name, Task[] commands) : base(name, commands)
    {
    }

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
