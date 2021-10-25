using System.Linq;
using Ares.Core;

namespace AresLib.Composers
{
  internal class StepComposer : CommandComposer<StepTemplate, StepExecutor>
  {
    public override StepExecutor Compose()
    {
      var commandCompilers =
        Template
          .CommandTemplates
          .Select
            (
             commandTemplate =>
             {
               var commandCompilerFactoryLookup = DeviceCommandCompilerFactoryRepoBridge.Repo.Lookup(commandTemplate.Metadata.DeviceName);
               var commandCompilerFactory = commandCompilerFactoryLookup.Value;
               var commandCompiler = commandCompilerFactory.Create(commandTemplate);
               return commandCompiler;
             }
            )
          .ToArray();

      var executables =
        commandCompilers
          .Select(cmdCompiler => cmdCompiler.Compile())
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
