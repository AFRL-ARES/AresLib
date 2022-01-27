using System.Collections.ObjectModel;
using Ares.Core.Executors;
using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Composers;

internal class ExperimentComposer : CommandComposer<ExperimentTemplate, ExperimentExecutor>
{
  public ExperimentComposer(
    ExperimentTemplate template,
    ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters)
  {
  }

  public override ExperimentExecutor Compose()
  {
    var stepCompilers =
      Template
        .StepTemplates
        .OrderBy(template => template.Index)
        .Select
        (
          stepTemplate =>
            new StepComposer(stepTemplate, AvailableDeviceCommandInterpreters)
        )
        .ToArray();

    var composedSteps = stepCompilers.Select(stepCompiler => stepCompiler.Compose());
    return new ExperimentExecutor(composedSteps.ToArray());
  }
}
