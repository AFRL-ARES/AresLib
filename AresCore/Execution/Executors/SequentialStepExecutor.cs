using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

internal class SequentialStepExecutor : StepExecutor
{
  public SequentialStepExecutor(StepTemplate template, Func<CancellationToken, Task<CommandResult>>[] commands) : base(template, commands)
  {
  }

  public override async Task<StepResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var commandResults = new List<CommandResult>();
    foreach (var command in Commands)
    {
      var commandResult = await command(cancellationToken);
      commandResults.Add(commandResult);
    }

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
