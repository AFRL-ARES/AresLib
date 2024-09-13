using Ares.Core.Analyzing;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors.Composers;

public class ExperimentComposer : ICommandComposer<ExperimentTemplate, ExperimentExecutor>
{
  private readonly ICommandComposer<StepTemplate, StepExecutor> _stepComposer;
  private readonly IAnalyzerManager _analyzerManager;

  public ExperimentComposer(ICommandComposer<StepTemplate, StepExecutor> stepComposer, IAnalyzerManager analyzerManager)
  {
    _stepComposer = stepComposer;
    _analyzerManager = analyzerManager;
  }

  public ExperimentExecutor Compose(ExperimentTemplate template)
  {
    var stepExecutors =
      template
        .StepTemplates
        .OrderBy(t => t.Index)
        .Select(_stepComposer.Compose)
        .ToArray();



    return new ExperimentExecutor(template, stepExecutors);
  }
}
