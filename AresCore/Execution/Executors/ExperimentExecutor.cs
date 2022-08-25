using System.Reactive.Linq;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class ExperimentExecutor : IExecutor<ExperimentResult, ExperimentExecutionStatus>
{

  public ExperimentExecutor(ExperimentTemplate template, StepExecutor[] stepExecutors)
  {
    StepExecutors = stepExecutors;
    Template = template;

    Status = new ExperimentExecutionStatus
    {
      ExperimentId = template.UniqueId
    };

    Status.StepExecutionStatuses.AddRange(stepExecutors.Select(executor => executor.Status));

    var stepExecutionObservation = stepExecutors.Select(executor => {
      return executor.StatusObservable.Select(_ => {
        var cmdResults = stepExecutors.Select(cmdExecutor => cmdExecutor.Status);
        Status.StepExecutionStatuses.Clear();
        Status.StepExecutionStatuses.AddRange(cmdResults);
        return Status;
      });
    }).Concat();

    StatusObservable = stepExecutionObservation;
  }


  public StepExecutor[] StepExecutors { get; }

  public ExperimentTemplate Template { get; set; }

  public IObservable<ExperimentExecutionStatus> StatusObservable { get; }
  public ExperimentExecutionStatus Status { get; }

  public async Task<ExperimentResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var stepResults = new List<StepResult>();
    foreach (var executableStep in StepExecutors)
    {
      if (cancellationToken.IsCancellationRequested)
        break;

      var stepResult = await executableStep.Execute(cancellationToken);
      stepResults.Add(stepResult);
    }

    return ExecutorResultHelpers.CreateExperimentResult(Template.UniqueId, startTime, DateTime.UtcNow, stepResults);
  }
}
