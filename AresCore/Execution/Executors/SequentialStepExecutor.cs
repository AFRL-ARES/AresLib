using Ares.Messaging;

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

      var commandResult = await command.Execute(cancellationToken);
      commandResults.Add(commandResult);
    }

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
