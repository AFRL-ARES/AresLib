using System.Reactive.Linq;
using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public abstract class StepExecutor : IExecutor<StepResult, StepExecutionStatus>
{
  public StepExecutor(StepTemplate template, IExecutor<CommandResult, CommandExecutionStatus>[] commandExecutors)
  {
    Template = template;
    CommandExecutors = commandExecutors;
    Status = new StepExecutionStatus
    {
      StepId = template.UniqueId,
      StepName = template.Name
    };

    Status.CommandExecutionStatuses.AddRange(commandExecutors.Select(executor => executor.Status));

    var commandExecutionObservation = commandExecutors.Select(executor => {
      return executor.ExperimentStatusObservable.Select(_ => {
        var cmdResults = commandExecutors.Select(cmdExecutor => cmdExecutor.Status);
        Status.CommandExecutionStatuses.Clear();
        Status.CommandExecutionStatuses.AddRange(cmdResults);
        return Status;
      });
    }).Concat();

    ExperimentStatusObservable = commandExecutionObservation;
  }

  public IExecutor<CommandResult, CommandExecutionStatus>[] CommandExecutors { get; }
  protected StepTemplate Template { get; }
  public IObservable<StepExecutionStatus> ExperimentStatusObservable { get; }
  public StepExecutionStatus Status { get; }
  public IObservable<StepExecutionStatus>? StartupStatusObservable { get; }
  public IObservable<StepExecutionStatus>? CloseoutStatusObservable { get; }
  public abstract Task<StepResult> Execute(ExecutionControlToken token);
}
