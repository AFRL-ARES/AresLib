using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepResult> Execute(ExecutionControlTokenSource tokenSource)
  {
    var startTime = DateTime.UtcNow;
    var commandTasks = CommandExecutors.Select(command => command.Execute(tokenSource));
    var commandResults = await Task.WhenAll(commandTasks);

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
