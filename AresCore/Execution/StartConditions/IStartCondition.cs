namespace Ares.Core.Execution.StartConditions;

public interface IStartCondition
{
  /// <summary>
  /// Gives a reason for why this start condition failed in case <see cref="CanStart" /> returns false
  /// </summary>
  public string Message { get; }

  /// <summary>
  /// Checks whether this start condition is satisfied
  /// </summary>
  /// <returns>True if condition is satisfied, false otherwise</returns>
  public bool CanStart();
}
