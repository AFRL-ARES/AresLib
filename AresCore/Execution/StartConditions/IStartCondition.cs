namespace Ares.Core.Execution.StartConditions;

public interface IStartCondition
{
  /// <summary>
  /// Should be bound to either a behavior subject as the backing source or some kind of publisher that emits periodically
  /// so that it doesn't get stuck on waiting for a status change
  /// </summary>
  public IObservable<bool> CanStartObservable { get; }
}
