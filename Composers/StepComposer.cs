using Ares.Core;
using System.Linq;

namespace AresLib
{
  internal class StepComposer : CommandComposer<StepTemplate>
  {
    public override ExecutableStep Compose()
    {
      var commandCompilers =
        Template
          .CommandTemplates
          .Select
            (
             commandTemplate =>
             {
               var commandCompilerFactoryLookup = DeviceCommandCompilerRepoBridge.Repo.Lookup(commandTemplate.Metadata.DeviceName);
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

      return new ExecutableStep { Commands = executables, IsParallel = Template.IsParallel, Name = Template.Name };
    }
  }
}
