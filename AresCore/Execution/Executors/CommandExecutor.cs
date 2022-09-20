using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class CommandExecutor : IExecutor<CommandResult, CommandExecutionStatus>
{
  private readonly Func<CancellationToken, Task<DeviceCommandResult>> _command;
  private readonly BehaviorSubject<CommandExecutionStatus> _stateSubject;

  public CommandExecutor(Func<CancellationToken, Task<DeviceCommandResult>> command, CommandTemplate template)
  {
    _command = command;
    Template = template;
    var executionStatus = new CommandExecutionStatus
    {
      CommandId = template.UniqueId,
      State = ExecutionState.Undefined
    };

    _stateSubject = new BehaviorSubject<CommandExecutionStatus>(executionStatus);
    StatusObservable = _stateSubject.AsObservable();
  }

  public CommandTemplate Template { get; set; }

  public IObservable<CommandExecutionStatus> StatusObservable { get; }
  public CommandExecutionStatus Status => _stateSubject.Value;

  public async Task<CommandResult> Execute(CancellationToken cancellationToken)
  {
    Status.State = ExecutionState.Running;
    _stateSubject.OnNext(Status);
    var timeStarted = DateTime.UtcNow;
    var execInfo = new ExecutionInfo { TimeStarted = DateTime.UtcNow.ToTimestamp() };
    var result = await _command(cancellationToken);
    execInfo.TimeFinished = DateTime.UtcNow.ToTimestamp();
    Status.State = ExecutionState.Succeeded;
    _stateSubject.OnNext(Status);
    _stateSubject.OnCompleted();
    return ExecutorResultHelpers.CreateCommandResult(Template.UniqueId, result, timeStarted, DateTime.UtcNow);
  }
}
