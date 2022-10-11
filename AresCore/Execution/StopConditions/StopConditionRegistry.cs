namespace Ares.Core.Execution.StopConditions;

internal class StopConditionRegistry
{
  private readonly IEnumerable<IStopCondition> _startConditions;

  public StopConditionRegistry(IEnumerable<IStopCondition> startConditions)
  {
    _startConditions = startConditions;
  }

  public IEnumerable<IStopCondition> GetFailedConditions()
  {
    var failedConditions = _startConditions.Where(condition => condition.ShouldStop());
    return failedConditions;
  }
}
