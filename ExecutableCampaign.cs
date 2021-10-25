using System.Threading.Tasks;
namespace AresLib
{
  internal class ExecutableCampaign : IBaseExecutable
  {
    public ExecutableExperiment[] Experiments { get; init; }

    public async Task Execute()
    {
      foreach (var executableExperiment in Experiments)
      {
        await executableExperiment.Execute();
      }
    }
  }
}
