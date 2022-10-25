namespace Ares.Core.Execution.StopConditions;

public class NumExperimentsRun : IStopCondition
{
  private readonly IExecutionReportStore _executionReportStore;
  private readonly uint _numExperiments;

  public NumExperimentsRun(IExecutionReportStore executionReportStore, uint numExperiments)
  {
    _executionReportStore = executionReportStore;
    _numExperiments = numExperiments;
  }

  public string Message => $"Stopped because {_executionReportStore.CampaignExecutionStatus?.ExperimentExecutionStatuses.Count}/{_numExperiments} experiments have been run";

  public bool ShouldStop()
  {
    var currentExperiments = _executionReportStore.CampaignExecutionStatus?.ExperimentExecutionStatuses.Count;
    return currentExperiments >= _numExperiments;
  }
}
