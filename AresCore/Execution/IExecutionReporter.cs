namespace Ares.Core.Execution;

public interface IExecutionReporter
{
  public void Report(ExecutionStatus status);
}
