using Ares.Core.Execution.Executors;
using Ares.Core.Planning;
using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Composers;

internal class ExperimentComposer : IExperimentComposer
{
  private readonly IEnumerable<IDeviceCommandInterpreter<IAresDevice>> _availableDeviceCommandInterpreters;
  private readonly IPlannerManager _plannerManager;

  public ExperimentComposer(
    IPlannerManager plannerManager,
    IEnumerable<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters)
  {
    _plannerManager = plannerManager;
    _availableDeviceCommandInterpreters = availableDeviceCommandInterpreters;
  }

  public ExperimentExecutor Compose(ExperimentTemplate template, IEnumerable<PlanSuggestion> planSuggestions)
  {
    var plannedArguments = _plannerManager.Plan(planSuggestions);
    var resolvedTemplate = ResolveTemplate(template, plannedArguments);
    return ComposeResolvedTemplate(resolvedTemplate);
  }

  public ExperimentExecutor Compose(ExperimentTemplate template)
    => ComposeResolvedTemplate(template);

  /// <summary>
  /// Copies an existing experiment template but with a new unique ID
  /// </summary>
  /// <param name="template"></param>
  /// <returns></returns>
  private ExperimentTemplate CopyTemplate(ExperimentTemplate template)
  {
    var templateCopy = template.Clone();
    templateCopy.UniqueId = Guid.NewGuid().ToString();
    return templateCopy;
  }

  private ExperimentTemplate ResolveTemplate(ExperimentTemplate template, IEnumerable<Parameter> resolvedArguments)
  {
    var templateCopy = CopyTemplate(template);
    var parametersToPlan = templateCopy.StepTemplates
      .SelectMany(stepTemplate => stepTemplate.CommandTemplates)
      .SelectMany(commandTemplate => commandTemplate.Arguments)
      .Where(argument => argument.Planned);

    var resolvedParametersEnumerated = resolvedArguments.ToArray();
    foreach (var unresolvedParameter in parametersToPlan)
    {
      var resolvedParameter = resolvedParametersEnumerated.First(resolvedParameter => resolvedParameter.Metadata.Name == unresolvedParameter.Metadata.Name);
      unresolvedParameter.Planned = false;
      unresolvedParameter.Value = resolvedParameter.Value;
    }

    return templateCopy;
  }

  private ExperimentExecutor ComposeResolvedTemplate(ExperimentTemplate resolvedTemplate)
  {
    var stepCompilers =
      resolvedTemplate
        .StepTemplates
        .OrderBy(template => template.Index)
        .Select
        (
          stepTemplate =>
            new StepComposer(stepTemplate, _availableDeviceCommandInterpreters)
        )
        .ToArray();

    var composedSteps = stepCompilers.Select(stepCompiler => stepCompiler.Compose());
    return new ExperimentExecutor(resolvedTemplate, composedSteps.ToArray());
  }
}
