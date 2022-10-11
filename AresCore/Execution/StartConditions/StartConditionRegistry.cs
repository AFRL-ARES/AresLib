namespace Ares.Core.Execution.StartConditions;

public class StartConditionRegistry : IStartConditionRegistry
{
  private readonly IEnumerable<IStartCondition> _startConditions;

  public StartConditionRegistry(IEnumerable<IStartCondition> startConditions)
  {
    _startConditions = startConditions;
  }

  public IEnumerable<IStartCondition> GetFailedConditions()
  {
    var failedConditions = _startConditions.Where(condition => !condition.CanStart());
    return failedConditions;
  }
}
