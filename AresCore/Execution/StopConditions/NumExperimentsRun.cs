namespace Ares.Core.Execution.StopConditions;

public class NumExperimentsRun : IStopCondition
{
  private readonly IExecutionReporter _executionReporter;
  private readonly uint _numExperiments;

  public NumExperimentsRun(IExecutionReporter executionReporter, uint numExperiments)
  {
    _executionReporter = executionReporter;
    _numExperiments = numExperiments;
  }

  public string Message => $"Stopped because {_executionReporter.CampaignExecutionStatus?.ExperimentExecutionStatuses.Count}/{_numExperiments} experiments have been run";

  public bool ShouldStop()
  {
    var currentExperiments = _executionReporter.CampaignExecutionStatus?.ExperimentExecutionStatuses.Count;
    return currentExperiments >= _numExperiments;
  }
}
