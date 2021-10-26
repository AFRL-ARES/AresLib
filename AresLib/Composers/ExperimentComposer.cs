using System.Linq;
using Ares.Core;
using AresLib.Executors;

namespace AresLib.Composers
{
  internal class ExperimentComposer : CommandComposer<ExperimentTemplate,ExperimentExecutor>
  {
    public override ExperimentExecutor Compose()
    {
      var stepCompilers =
        Template
          .StepTemplates
          .Select
            (
             stepTemplate =>
               new StepComposer
               {
                 Template = stepTemplate,
                 CommandNamesToInterpreters = CommandNamesToInterpreters
               }
            )
          .ToArray();

      var composedSteps = stepCompilers.Select(stepCompiler => stepCompiler.Compose());
      return new ExperimentExecutor { Steps = composedSteps.ToArray() };
    }
  }
}
