using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ares.Core.Execution.Executors;

public class CommandExecutor : IExecutor<CommandResult, CommandExecutionStatus>
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
      CommandName = template.Metadata.Name,
      DeviceName = template.Metadata.DeviceName,
      State = ExecutionState.Undefined
    };

    _stateSubject = new BehaviorSubject<CommandExecutionStatus>(executionStatus);

    ExperimentStatusObservable = _stateSubject.AsObservable();
  }

  public CommandTemplate Template { get; set; }

  public IObservable<CommandExecutionStatus> ExperimentStatusObservable { get; }
  public CommandExecutionStatus Status => _stateSubject.Value;
  public async Task<CommandResult> Execute(ExecutionControlTokenSource tokenSource)
  {
    var token = tokenSource.Token;

    Status.State = token.IsPaused ? ExecutionState.Paused : ExecutionState.Running;
    _stateSubject.OnNext(Status);
    if(token.IsPaused)
      try
      {
        token.WaitForResume(token.CancellationToken);
      }
      catch(OperationCanceledException)
      {
      }

    if(token.IsCancelled)
    {
      Status.State = ExecutionState.Failed;
      _stateSubject.OnNext(Status);
      _stateSubject.OnCompleted();
      return ExecutorResultHelpers.CreateCommandResult(Template, null, DateTime.UtcNow, DateTime.UtcNow);
    }

    var timeStarted = DateTime.UtcNow;
    var execInfo = new ExecutionInfo { TimeStarted = DateTime.UtcNow.ToTimestamp() };
    var result = await InternalExecute(token.CancellationToken);
    execInfo.TimeFinished = DateTime.UtcNow.ToTimestamp();

    if(result.AwaitUserInput)
      AwaitUserInput(tokenSource);


    else if(result.Success)
      Status.State = ExecutionState.Succeeded;

    else
      Status.State = ExecutionState.Failed;

    _stateSubject.OnNext(Status);
    _stateSubject.OnCompleted();

    return ExecutorResultHelpers.CreateCommandResult(Template, result, timeStarted, timeStarted);
  }

  private async Task<DeviceCommandResult> InternalExecute(CancellationToken token)
  {
    try
    {
      var result = await _command(token);
      return result;
    }
    catch(Exception e)
    {
      var result = new DeviceCommandResult() { Success = false, Error = e.Message };
      return result;
    }
  }

  private void AwaitUserInput(ExecutionControlTokenSource tokenSource)
  {
    tokenSource.Pause();
    Status.State = ExecutionState.AwaitingUser;
    _stateSubject.OnNext(Status);
    var ct = new CancellationToken();
    tokenSource.WaitForResume(ct);
    Status.State = ExecutionState.Succeeded;
  }
}
