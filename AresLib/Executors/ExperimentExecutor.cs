using System;
using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal class ExperimentExecutor : IBaseExecutor
  {
    public ExperimentExecutor(StepExecutor[] stepExecutors)
    {
      StepExecutors = stepExecutors;
    }
    public StepExecutor[] StepExecutors { get; }
    public async Task Execute()
    {
      foreach (var executableStep in StepExecutors)
      {
        Console.WriteLine($"Started step {executableStep.Name}");
        var startTime = DateTime.Now;
        await executableStep.Execute();
        var endTime = DateTime.Now;
        var duration = endTime - startTime;
        Console.WriteLine($"{executableStep.Name} Step duration: {duration.TotalSeconds} seconds");
      }
    }
  }
}
