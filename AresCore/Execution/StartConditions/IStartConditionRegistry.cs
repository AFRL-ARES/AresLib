namespace Ares.Core.Execution.StartConditions;

public interface IStartConditionRegistry
{
  public IObservable<bool> CanStart { get; }
}
