namespace Ares.Core.Execution.StopConditions;

public interface IStopConditionRegistry
{
  public IEnumerable<IStopCondition> GetFailedConditions();
}
