using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class SequentialStepExecutor : StepExecutor
{
  public SequentialStepExecutor(StepTemplate template, CommandExecutor[] commandExecutors) : base(template, commandExecutors)
  {
  }

  public override async Task<StepExecutionSummary> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var CommandSummaries = new List<CommandExecutionSummary>();
    foreach (var command in CommandExecutors)
    {
      if (token.IsCancelled)
        break;

      var CommandExecutionSummary = await command.Execute(token);

      if(CommandExecutionSummary.Result.Success)
        CommandSummaries.Add(CommandExecutionSummary);

      else
        return ExecutorSummaryHelpers.CreateEmptyStepExecutionSummary(Template.UniqueId, startTime, DateTime.UtcNow);
    }

    return ExecutorSummaryHelpers.CreateStepExecutionSummary(Template.UniqueId, startTime, DateTime.UtcNow, CommandSummaries);
  }
}
