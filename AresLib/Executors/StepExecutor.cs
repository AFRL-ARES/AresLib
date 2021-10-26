using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal abstract class StepExecutor : IBaseExecutor
  {
    public Task[] Commands { get; init; }
    public string Name { get; init; }

    public abstract Task Execute();
  }
}
