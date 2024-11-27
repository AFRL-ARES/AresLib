using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class SequentialStepExecutor : StepExecutor
{
  public SequentialStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepResult> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var commandResults = new List<CommandResult>();
    foreach (var command in CommandExecutors)
    {
      if (token.IsCancelled)
        break;

      var commandResult = await command.Execute(token);

      if(commandResult.Result.Success)
        commandResults.Add(commandResult);

      else
        return ExecutorResultHelpers.CreateEmptyStepResult(Template.UniqueId, startTime, DateTime.UtcNow);
    }

    return ExecutorResultHelpers.CreateStepResult(Template.UniqueId, startTime, DateTime.UtcNow, commandResults);
  }
}
