namespace Ares.Core.Execution.StopConditions;

internal class NumExperimentsRunFactory : INumExperimentsRunFactory
{
  private readonly IExecutionReporter _executionReporter;

  public NumExperimentsRunFactory(IExecutionReporter executionReporter)
  {
    _executionReporter = executionReporter;
  }

  public NumExperimentsRun Create(uint numExperiments)
    => new(_executionReporter, numExperiments);
}
