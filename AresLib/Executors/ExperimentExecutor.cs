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
      for (int i = 0; i < StepExecutors.Length; i++)
      {
        var executableStep = StepExecutors[i];
        Console.WriteLine($"Started step {i}: {executableStep.Name}");
        var startTime = DateTime.Now;
        await executableStep.Execute();
        var endTime = DateTime.Now;
        var duration = endTime - startTime;
        Console.WriteLine($"{executableStep.Name} Step {i} duration: {duration.TotalSeconds} seconds");
      }
    }
  }
}
