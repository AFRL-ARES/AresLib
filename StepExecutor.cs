using System;
using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class StepExecutor : IBaseExecutor
  {
    public Task[] Commands { get; init; }
    public string Name { get; init; }

    public abstract Task Execute();
  }
}
