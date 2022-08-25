using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class ParallelStepExecutor : StepExecutor
{
  public ParallelStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var commandTasks = CommandExecutors.Select(command => command.Execute(cancellationToken));
    var commandResults = await Task.WhenAll(commandTasks);

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }

  private static async Task<CommandResult> ExecuteDeviceCommand(Func<CancellationToken, Task> command, CancellationToken cancellationToken)
  {
    var executionInfo = new ExecutionInfo { TimeStarted = DateTime.UtcNow.ToTimestamp() };
    await command(cancellationToken);
    executionInfo.TimeFinished = DateTime.UtcNow.ToTimestamp();
    return new CommandResult
    {
      CommandId = Guid.NewGuid().ToString(),
      ExecutionInfo = executionInfo
    };
  }
}
