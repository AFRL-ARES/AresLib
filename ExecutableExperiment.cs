using System.Threading.Tasks;
namespace AresLib
{
  internal class ExecutableExperiment : IBaseExecutable
  {
    public ExecutableStep[] Steps { get; init; }
    public async Task Execute()
    {
      foreach (var executableStep in Steps)
      {
        await executableStep.Execute();
      }
    }
  }
}
