using System.Linq;
using Ares.Core;
using AresLib.Executors;

namespace AresLib.Composers
{
  internal class StepComposer : CommandComposer<StepTemplate, StepExecutor>
  {
    public override StepExecutor Compose()
    {
      var executables =
        Template
          .CommandTemplates
          .Select
            (
             commandTemplate =>
             {
               var commandInterpreter = CommandNamesToInterpreters[StaticStuff.QualifyCommandName(commandTemplate.Metadata)];
               var executable = commandInterpreter.TemplateToDeviceCommand(commandTemplate);
               return executable;
             }
            )
          .ToArray();

      return Template.IsParallel
        ? new ParallelStepExecutor
          {
            Commands = executables,
            Name = Template.Name
          }
        : new SequentialStepExecutor
          {
            Commands = executables,
            Name = Template.Name
          };
    }
  }
}
