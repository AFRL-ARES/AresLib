using Ares.Core;
using System.Linq;

namespace AresLib
{
  internal class ExperimentComposer : CommandComposer<ExperimentTemplate>
  {
    public override ExecutableExperiment Compose()
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
                 DeviceCommandCompilerRepoBridge = DeviceCommandCompilerRepoBridge
               }
            )
          .ToArray();

      var composedSteps = stepCompilers.Select(stepCompiler => stepCompiler.Compose());
      return new ExecutableExperiment { Steps = composedSteps.ToArray() };
    }
  }
}
