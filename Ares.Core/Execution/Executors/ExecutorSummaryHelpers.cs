using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal static class ExecutorSummaryHelpers
{
  public static ExperimentExecutionSummary CreateExperimentExecutionSummary(string experimentId,
    CompletedExperiment completedExperiment,
    DateTime startTime,
    DateTime endTime,
    IEnumerable<StepExecutionSummary> StepSummaries)
  {
    var experimentSummary = new ExperimentExecutionSummary
    {
      UniqueId = Guid.NewGuid().ToString(),
      ExecutionInfo = MakeExecutionInfo(startTime, endTime),
      ExperimentId = experimentId,
      CompletedExperiment = completedExperiment,
    };

    experimentSummary.StepSummaries.AddRange(StepSummaries);
    return experimentSummary;
  }

  public static StepExecutionSummary CreateStepExecutionSummary(string stepId,
    DateTime startTime,
    DateTime endTime,
    IEnumerable<CommandExecutionSummary> CommandSummaries)
  {
    var stepResult = new StepExecutionSummary
    {
      UniqueId = Guid.NewGuid().ToString(),
      ExecutionInfo = MakeExecutionInfo(startTime, endTime),
      StepId = stepId
    };

    stepResult.CommandSummaries.AddRange(CommandSummaries);

    return stepResult;
  }

  public static StepExecutionSummary CreateEmptyStepExecutionSummary(string stepId, DateTime startTime, DateTime endTime)
  {
    return new StepExecutionSummary { UniqueId = Guid.NewGuid().ToString(), ExecutionInfo = MakeExecutionInfo(startTime, endTime) };
  }
  public static CommandExecutionSummary CreateCommandExecutionSummary(string commandId,
    DeviceCommandResult? deviceResult,
    DateTime startTime,
    DateTime endTime)
  {
    var CommandExecutionSummary = new CommandExecutionSummary
    {
      UniqueId = Guid.NewGuid().ToString(),
      ExecutionInfo = MakeExecutionInfo(startTime, endTime),
      CommandId = commandId,
      Result = deviceResult
    };

    return CommandExecutionSummary;
  }

  private static ExecutionInfo MakeExecutionInfo(DateTime startTime, DateTime endTime)
    => new()
    {
      UniqueId = Guid.NewGuid().ToString(),
      TimeFinished = endTime.ToTimestamp(),
      TimeStarted = startTime.ToTimestamp()
    };
}
