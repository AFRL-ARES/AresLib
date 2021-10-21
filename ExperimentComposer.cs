using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal class ExperimentComposer : CommandComposer<ExperimentTemplate>
  {
    public override Task Compose()
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
        return composedSteps
          .Aggregate(
                   async (current, next) =>
                   {
                     await current;
                     await next;
                   });
    } 
  }
}
