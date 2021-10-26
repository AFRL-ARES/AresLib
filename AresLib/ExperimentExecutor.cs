using System;
using System.Threading.Tasks;
namespace AresLib
{
  internal class ExperimentExecutor : IBaseExecutor
  {
    public StepExecutor[] Steps { get; init; }
    public async Task Execute()
    {
      foreach (var executableStep in Steps)
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
