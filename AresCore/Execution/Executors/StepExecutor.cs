using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Executors;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public abstract class StepExecutor : IBaseExecutor<StepResult, StepTemplate>
{
  protected readonly ISubject<ExecutorState> _stateSubject = new BehaviorSubject<ExecutorState>(ExecutorState.Idle);

  public StepExecutor(StepTemplate template, Func<CancellationToken, Task>[] commands)
  {
    Template = template;
    Commands = commands;
    State = _stateSubject.AsObservable();
  }

  public Func<CancellationToken, Task>[] Commands { get; }
  public IObservable<ExecutorState> State { get; }
  public StepTemplate Template { get; set; }

  public abstract Task<StepResult> Execute(CancellationToken cancellationToken);
}
