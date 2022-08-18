using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Executors;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class ExperimentExecutor : IBaseExecutor<ExperimentResult, ExperimentTemplate>
{
  private readonly ISubject<ExecutorState> _stateSubject = new BehaviorSubject<ExecutorState>(ExecutorState.Idle);

  public ExperimentExecutor(ExperimentTemplate template, StepExecutor[] stepExecutors)
  {
    StepExecutors = stepExecutors;
    Template = template;
    State = _stateSubject.AsObservable();
  }

  public IObservable<ExecutorState> State { get; }

  public StepExecutor[] StepExecutors { get; }

  public ExperimentTemplate Template { get; set; }

  public async Task<ExperimentResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var stepResults = new List<StepResult>();
    foreach (var executableStep in StepExecutors)
    {
      var stepResult = await executableStep.Execute(cancellationToken);
      stepResults.Add(stepResult);
    }

    return ExecutorResultHelpers.CreateExperimentResult(Template.UniqueId, startTime, DateTime.UtcNow, stepResults);
  }
}
