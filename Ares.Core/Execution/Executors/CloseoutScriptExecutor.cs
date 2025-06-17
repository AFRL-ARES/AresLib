using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;

namespace Ares.Core.Execution.Executors
{
  public class CloseoutScriptExecutor : IExecutor<Empty, CampaignCloseoutStatus>
  {
    public CloseoutScriptExecutor(ExperimentTemplate template, IExecutor<StepResult, StepExecutionStatus>[] closeoutStepExecutors)
    {
      CloseoutStepExecutors = closeoutStepExecutors;
      Template = template;
      Status = new CampaignCloseoutStatus { CampaignId = template.UniqueId };

      Status.CloseoutExecutionStatuses.AddRange(closeoutStepExecutors.Select(executor => executor.Status));

      var experimentStepExecutionObservation = closeoutStepExecutors.Select(executor =>
      {
        return executor.ExperimentStatusObservable.Select(_ =>
        {
          var cmdResults = closeoutStepExecutors.Select(cmdExecutor => cmdExecutor.Status);
          Status.CloseoutExecutionStatuses.Clear();
          Status.CloseoutExecutionStatuses.AddRange(cmdResults);
          return Status;
        });
      }).Concat();

      ExperimentStatusObservable = experimentStepExecutionObservation;
    }

    public IObservable<CampaignCloseoutStatus> ExperimentStatusObservable { get; }

    public IExecutor<StepResult, StepExecutionStatus>[] CloseoutStepExecutors { get; }

    public CampaignCloseoutStatus Status { get; }

    public ExperimentTemplate Template { get; set; }


    public async Task<Empty> Execute(ExecutionControlTokenSource executionTokenSource)
    {
      foreach(var closeoutStep in CloseoutStepExecutors)
      {
        if(executionTokenSource.Token.IsCancelled)
          break;

        var stepResult = await closeoutStep.Execute(executionTokenSource);

        if(!stepResult.CommandResults.Any())
          break;
      }

      return new Empty();
    }
  }
}
