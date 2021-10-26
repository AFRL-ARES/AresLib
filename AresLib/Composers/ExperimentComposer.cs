using System.Linq;
using Ares.Core;

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
                 DeviceCommandCompilerFactoryRepoBridge = DeviceCommandCompilerFactoryRepoBridge
               }
            )
          .ToArray();

      var composedSteps = stepCompilers.Select(stepCompiler => stepCompiler.Compose());
      return new ExperimentExecutor { Steps = composedSteps.ToArray() };
    }
  }
}
