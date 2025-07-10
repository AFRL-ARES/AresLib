using System.Reactive.Linq;
using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Extensions;
using Ares.Core.Helpers;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors;

public class ExperimentExecutor : IExecutor<ExperimentExecutionSummary, ExperimentExecutionStatus>
{

  public ExperimentExecutor(ExperimentTemplate template,
    IExecutor<StepExecutionSummary, StepExecutionStatus>[] experimentStepExecutors)
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


  public IExecutor<StepExecutionSummary, StepExecutionStatus>[] ExperimentStepExecutors { get; }

  public ExperimentTemplate Template { get; set; }

  public IObservable<ExperimentExecutionStatus> ExperimentStatusObservable { get; }

  public ExperimentExecutionStatus Status { get; }

  public async Task<ExperimentExecutionSummary> Execute(ExecutionControlToken token)
  {
    var startTime = DateTime.UtcNow;
    var stepSummaries = new List<StepExecutionSummary>();
    foreach(var executableStep in ExperimentStepExecutors)
    {
      if(token.IsCancelled)
        break;

      var stepResult = await executableStep.Execute(token);

      if(!stepResult.CommandSummaries.Any())
        break;

      stepSummaries.Add(stepResult);
    }

    var completedExperiment = new CompletedExperiment
    {
      Template = Template
    };

    completedExperiment.Parameters.AddRange(Template.GetAllPlannedParameters());



    if(Template.OutputCommands.Any())
    {
      var keyedCommandSummaries = stepSummaries
        .SelectMany(stepSummary => stepSummary.CommandSummaries)
        .Select(
          (summary) => new
          {
            Summary = summary,
            Template.OutputCommands.FirstOrDefault(oc => oc.CommandId == summary.CommandId)?.Key
          })
        .Where(anon => anon.Key is not null);


      var experimentResult = new AresStruct();

      foreach(var summary in keyedCommandSummaries)
      {
        var cmdResult = summary.Summary.Result.Result;
        experimentResult.AddValue(summary.Key!, cmdResult);
      }

      completedExperiment.Result = experimentResult;
    }

    return ExecutorSummaryHelpers.CreateExperimentExecutionSummary(Template.UniqueId, completedExperiment, startTime, DateTime.UtcNow, stepSummaries);
  }
}
