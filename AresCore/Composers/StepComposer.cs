using Ares.Core.Execution.Executors;
using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Composers;

internal class StepComposer : ICommandComposer<StepTemplate, StepExecutor>
{
  private readonly IEnumerable<IDeviceCommandInterpreter<IAresDevice>> _availableDeviceCommandInterpreters;

  public StepComposer(IEnumerable<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters)
  {
    _availableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
  }

  public StepExecutor Compose(StepTemplate template)
  {
    var executables =
      template
        .CommandTemplates
        .OrderBy(t => t.Index)
        .Select
        (
          commandTemplate => {
            var commandInterpreter =
              _availableDeviceCommandInterpreters
                .First(interpreter =>
                  interpreter
                    .Device
                    .Name
                    .Equals(commandTemplate.Metadata.DeviceName));

            var executable = commandInterpreter.TemplateToDeviceCommand(commandTemplate);
            return executable;
          }
        )
        .ToArray();


    return template.IsParallel
      ? new ParallelStepExecutor(template, executables)
      : new SequentialStepExecutor(template, executables);
  }
}
