using Ares.Core;
using System.Linq;
using System.Threading.Tasks;

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

      return
        Template.IsParallel
          ? Task.WhenAll(executables)
          : executables
            .Aggregate(
                       async (current, next) =>
                       {
                         await current;
                         await next;
                       });
    }
  }
}
