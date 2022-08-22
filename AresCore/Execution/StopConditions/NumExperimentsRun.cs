namespace Ares.Core.Execution;

public class NumExperimentsRun : IStopCondition
{

  public Func<bool> ShouldStop()
    => () => false;
}
