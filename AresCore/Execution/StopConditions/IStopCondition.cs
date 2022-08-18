namespace Ares.Core.Execution;

public interface IStopCondition
{
  Func<bool> ShouldStop();
}
