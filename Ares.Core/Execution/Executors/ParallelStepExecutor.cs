using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepExecutionSummary> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var commandTasks = CommandExecutors.Select(command => command.Execute(token));
    var CommandSummaries = await Task.WhenAll(commandTasks);

    return ExecutorSummaryHelpers.CreateStepExecutionSummary(Template.UniqueId, startTime, DateTime.UtcNow, CommandSummaries);
  }
}
