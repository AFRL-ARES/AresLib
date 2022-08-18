using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal static class ExecutorResultHelpers
{
  public static ExperimentResult CreateExperimentResult(string experimentId, DateTime startTime, DateTime endTime, IEnumerable<StepResult> stepResults)
  {
    var experimentResult = new ExperimentResult
    {
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = endTime.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      },
      ExperimentId = experimentId
    };
    experimentResult.StepResults.AddRange(stepResults);
    
    return experimentResult;
  }

  public static StepResult CreateStepResult(string stepId,
    DateTime startTime,
    DateTime endTime,
    IEnumerable<CommandResult> commandResults)
  {
    var stepResult = new StepResult
    {
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = endTime.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      },
      StepId = stepId
    };

    stepResult.CommandResults.AddRange(commandResults);

    return stepResult;
  }

  public static CommandResult CreateCommandResult(string commandId,
    DateTime startTime,
    DateTime endTime)
  {
    var commandResult = new CommandResult
    {
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = endTime.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      },
      CommandId = commandId
    };
    
    return commandResult;
  }
}
