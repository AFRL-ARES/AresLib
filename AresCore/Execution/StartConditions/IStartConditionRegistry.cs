namespace Ares.Core.Execution.StartConditions;

public interface IStartConditionRegistry
{
  public IEnumerable<IStartCondition> GetFailedConditions();
}
