using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Extensions;
using Ares.Messaging;
using System.Reactive.Linq;

namespace Ares.Core.Execution.Executors;

public class ExperimentExecutor : IExecutor<ExperimentResult, ExperimentExecutionStatus>
{

  public ExperimentExecutor(ExperimentTemplate template,
    IExecutor<StepResult, StepExecutionStatus>[] experimentStepExecutors)
  {
    ExperimentStepExecutors = experimentStepExecutors;
    Template = template;

    Status = new ExperimentExecutionStatus
    {
      ExperimentId = template.UniqueId
    };

    Status.StepExecutionStatuses.AddRange(experimentStepExecutors.Select(executor => executor.Status));

    var experimentStepExecutionObservation = experimentStepExecutors.Select(executor =>
    {
      return executor.ExperimentStatusObservable.Select(_ =>
      {
        var cmdResults = experimentStepExecutors.Select(cmdExecutor => cmdExecutor.Status);
        Status.StepExecutionStatuses.Clear();
        Status.StepExecutionStatuses.AddRange(cmdResults);
        return Status;
      });
    }).Concat();

    ExperimentStatusObservable = experimentStepExecutionObservation;
  }


  public IExecutor<StepResult, StepExecutionStatus>[] ExperimentStepExecutors { get; }

  public ExperimentTemplate Template { get; set; }

  public IObservable<ExperimentExecutionStatus> ExperimentStatusObservable { get; }

  public ExperimentExecutionStatus Status { get; }

  public async Task<ExperimentResult> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var stepResults = new List<StepResult>();
    foreach(var executableStep in ExperimentStepExecutors)
    {
      if(token.IsCancelled)
        break;

      var stepResult = await executableStep.Execute(token);

      if(!stepResult.CommandResults.Any())
        break;

      stepResults.Add(stepResult);
    }

    var completedExperiment = new CompletedExperiment
    {
      Template = Template
    };

    completedExperiment.Parameters.AddRange(Template.GetAllPlannedParameters());

    if(!string.IsNullOrEmpty(Template.OutputCommandId))
    {
      var commandResult = stepResults.SelectMany(stepResult => stepResult.CommandResults).FirstOrDefault(cmdResult => cmdResult.CommandId == Template.OutputCommandId);
      completedExperiment.Result = commandResult?.Result.Result;
    }

    return ExecutorResultHelpers.CreateExperimentResult(Template.UniqueId, completedExperiment, startTime, DateTime.UtcNow, stepResults);
  }
}
