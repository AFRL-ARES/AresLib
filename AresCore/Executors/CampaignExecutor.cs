namespace Ares.Core.Executors;

internal class CampaignExecutor : IBaseExecutor
{
  public CampaignExecutor(ExperimentExecutor[] experimentExecutors)
  {
    ExperimentExecutors = experimentExecutors;
  }

  public ExperimentExecutor[] ExperimentExecutors { get; }

  public async Task Execute()
  {
    foreach (var executableExperiment in ExperimentExecutors)
      await executableExperiment.Execute();
  }
}
