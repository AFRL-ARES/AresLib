namespace Ares.Core.Execution.StartConditions;

public interface IStartCondition
{
  public string Message { get; }
  public bool CanStart();
}
