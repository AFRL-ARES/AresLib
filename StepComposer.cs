using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib
{
  internal class StepComposer : CommandComposer<StepTemplate>
  {
    public override Task Compose()
    {
      var commandCompilers =
        Template
          .CommandTemplates
          .Select
            (
             commandTemplate =>
             {
               var commandCompilerFactoryLookup = DeviceCommandCompilerRepoBridge.Repo.Lookup(commandTemplate.Metadata.Name);
               var commandCompilerFactory = commandCompilerLookup.Value;
               var commandCompiler = commandCompilerFactory.Compile(commandTemplate);
               return commandCompiler;
             }
            )
          .ToArray();

      var 
        commandCompilers
          .Select(cmdCompiler => cmdCompiler.GenerateExecutable())
          .ToArray();

      return 
        Template.IsParallel
          ? Task.WhenAll(commandExecutables)
          : commandExecutables
            .Aggregate(
                       async (current, next) =>
                       {
                         await current;
                         await next;
                       });
    }
  }
}
