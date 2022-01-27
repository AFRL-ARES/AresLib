using System.Collections.ObjectModel;
using Ares.Core.Executors;
using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Composers;

internal class StepComposer : CommandComposer<StepTemplate, StepExecutor>
{
  public StepComposer(StepTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters)
  {
  }

  public override StepExecutor Compose()
  {
    var executables =
      Template
        .CommandTemplates
        .OrderBy(template => template.Index)
        .Select
        (
          commandTemplate => {
            var commandInterpreter =
              AvailableDeviceCommandInterpreters
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


    return Template.IsParallel
      ? new ParallelStepExecutor(Template.Name, executables)
      : new SequentialStepExecutor(Template.Name, executables);
  }
}
