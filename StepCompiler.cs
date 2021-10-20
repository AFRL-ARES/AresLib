using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal class StepCompiler : ExecutableCompiler, IStepCompiler
  {

    public override Task GenerateExecutable()
    {
      var commandCompilers = 
        Step
          .Commands
          .Select(command =>
                    new CommandCompiler
                    {
                      Command = command, 
                      DeviceTranslatorRepoBridge = DeviceTranslatorRepoBridge
                    }
                 )
          .ToArray();

      var commandExecutables = 
        commandCompilers
          .Select(cmdCompiler => cmdCompiler.GenerateExecutable())
          .ToArray();

      return Step.IsParallel
        ? Task.WhenAll(commandExecutables)
        : commandExecutables
          .Aggregate(
                     async (current, next) =>
                     {
                       await current;
                       await next;
                     });
    }

    public ExperimentStep Step { get; init; }
  }
}
