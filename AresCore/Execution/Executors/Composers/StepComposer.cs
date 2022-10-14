using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors.Composers;

public class StepComposer : ICommandComposer<StepTemplate, StepExecutor>
{
  private readonly IEnumerable<IDeviceCommandInterpreter<IAresDevice>> _availableDeviceCommandInterpreters;

  public StepComposer(ILaboratoryManager laboratoryManager)
  {
    _availableDeviceCommandInterpreters = laboratoryManager.Lab.DeviceInterpreters;
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

            var command = commandInterpreter.TemplateToDeviceCommand(commandTemplate);
            var executable = new CommandExecutor(command, commandTemplate);
            return executable;
          }
        )
        .ToArray();


    return template.IsParallel
      ? new ParallelStepExecutor(template, executables)
      : new SequentialStepExecutor(template, executables);
  }
}
