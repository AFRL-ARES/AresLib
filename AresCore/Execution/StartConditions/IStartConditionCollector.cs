namespace Ares.Core.Execution.StartConditions;

public interface IStartConditionCollector
{
  public IObservable<bool> CanStart { get; }
}
