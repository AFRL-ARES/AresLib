using Ares.Core.Execution.ControlTokens;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;

namespace Ares.Core.Execution.Executors
{
  public class StartupScriptExecutor : IExecutor<Empty, CampaignStartupStatus>
  {
    public StartupScriptExecutor(ExperimentTemplate template,
    IExecutor<StepExecutionSummary, StepExecutionStatus>[] startupStepExecutors)
    {
      StartupStepExecutors = startupStepExecutors;
      Template = template;
      Status = new CampaignStartupStatus { CampaignId = template.UniqueId };

      Status.StartupExecutionStatuses.AddRange(startupStepExecutors.Select(executor => executor.Status));

      var experimentStepExecutionObservation = startupStepExecutors.Select(executor =>
      {
        return executor.ExperimentStatusObservable.Select(_ =>
        {
          var cmdResults = startupStepExecutors.Select(cmdExecutor => cmdExecutor.Status);
          Status.StartupExecutionStatuses.Clear();
          Status.StartupExecutionStatuses.AddRange(cmdResults);
          return Status;
        });
      }).Concat();

      ExperimentStatusObservable = experimentStepExecutionObservation;
    }

    public IObservable<CampaignStartupStatus> ExperimentStatusObservable { get; }
    public CampaignStartupStatus Status { get; }
    public IExecutor<StepExecutionSummary, StepExecutionStatus>[] StartupStepExecutors { get; }
    public ExperimentTemplate Template { get; set; }

    public async Task<Empty> Execute(ExecutionControlTokenSource executionTokenSource)
    {

      foreach(var startupStep in StartupStepExecutors)
      {
        if(executionTokenSource.Token.IsCancelled)
          break;

        var stepResult = await startupStep.Execute(executionTokenSource);

        if(!stepResult.CommandSummaries.Any())
          break;
      }

      return new Empty();
    }
  }
}
