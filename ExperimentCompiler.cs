using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal class ExperimentCompiler : ExecutableCompiler, IExperimentCompiler
  {
    public override Task GenerateExecutable()
    {
      var stepCompilers =
        Experiment
          .Steps
          .Select
            (
             step =>
               new StepCompiler
               {
                 Step = step,
                 DeviceTranslatorRepoBridge = DeviceTranslatorRepoBridge
               }
            )
          .ToArray();

        var executableSteps = stepCompilers.Select(stepCompiler => stepCompiler.GenerateExecutable());
        return executableSteps
          .Aggregate(
                   async (current, next) =>
                   {
                     await current;
                     await next;
                   });
    } 

    public Experiment Experiment { get; init; }
  }
}
