using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class SequentialStepExecutor : StepExecutor
{
  public SequentialStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var commandResults = new List<CommandResult>();
    foreach (var command in CommandExecutors)
    {
      if (cancellationToken.IsCancellationRequested)
        break;

      var executionInfo = new ExecutionInfo { TimeStarted = DateTime.UtcNow.ToTimestamp() };
      await command.Execute(cancellationToken);
      executionInfo.TimeFinished = DateTime.UtcNow.ToTimestamp();
      var commandResult = new CommandResult
      {
        CommandId = Guid.NewGuid().ToString(),
        ExecutionInfo = executionInfo
      };

      commandResults.Add(commandResult);
    }

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
