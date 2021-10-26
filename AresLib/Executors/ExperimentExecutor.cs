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
        var startTime = DateTime.Now;
        await executableStep.Execute();
        var endTime = DateTime.Now;
        var duration = endTime - startTime;
        Console.WriteLine($"Step {executableStep.Name} duration: {duration.TotalSeconds} seconds");
      }
    }
  }
}
