using System.Collections.ObjectModel;
using System.Linq;
using Ares.Core.Messages;
using AresDevicePluginBase;
using AresLib.Executors;

namespace AresLib.Composers
{
  internal class ExperimentComposer : CommandComposer<ExperimentTemplate,ExperimentExecutor>
  {
    public ExperimentComposer(
      ExperimentTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters) { }

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
}
