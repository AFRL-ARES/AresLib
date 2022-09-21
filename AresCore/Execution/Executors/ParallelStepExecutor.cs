using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

internal class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepResult> Execute(CancellationToken cancellationToken, PauseToken pauseToken)
  {
    var startTime = DateTime.UtcNow;
    var commandTasks = CommandExecutors.Select(command => command.Execute(cancellationToken, pauseToken));
    var commandResults = await Task.WhenAll(commandTasks);

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
