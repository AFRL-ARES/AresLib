using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

internal class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(StepTemplate template, Func<CancellationToken, Task<CommandResult>>[] commands) : base(template, commands)
  {
  }

  public override async Task<StepResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var commandTasks = Commands.Select(func => func(cancellationToken));
    var commandResults = await Task.WhenAll(commandTasks);
    
    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
