using System.Threading.Tasks;

namespace AresLib.Executors
{
  internal class CampaignExecutor : IBaseExecutor
  {
    public ExperimentExecutor[] Experiments { get; init; }

    public async Task Execute()
    {
      foreach (var executableExperiment in Experiments)
      {
        await executableExperiment.Execute();
      }
    }
  }
}
