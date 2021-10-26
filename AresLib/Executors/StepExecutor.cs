using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal abstract class StepExecutor : IBaseExecutor
  {
    public StepExecutor(string name, Task[] commands)
    {
      Name = name;
      Commands = commands;
    }
    public Task[] Commands { get; }
    public string Name { get; }

    public abstract Task Execute();
  }
}
