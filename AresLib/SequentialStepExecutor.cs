using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class SequentialStepExecutor : StepExecutor
  {
    public override async Task Execute()
    {
      Console.WriteLine($"Executing Step: {Name}");
      foreach (var command in Commands)
      {
        command.Start();
        await command;
      }
    }
  }
}
