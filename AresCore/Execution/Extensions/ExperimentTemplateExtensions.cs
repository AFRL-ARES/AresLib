using Ares.Messaging;

namespace Ares.Core.Execution.Extensions;

internal static class ExperimentTemplateExtensions
{
  public static IEnumerable<Parameter> GetAllParameters(this ExperimentTemplate template)
    => template.StepTemplates
      .SelectMany(stepTemplate => stepTemplate.CommandTemplates)
      .SelectMany(commandTemplate => commandTemplate.Arguments);

  public static IEnumerable<Parameter> GetAllPlannedParameters(this ExperimentTemplate template)
    => template.GetAllParameters().Where(parameter => parameter.Planned);

  public static bool IsResolved(this ExperimentTemplate template)
    => template.GetAllParameters().All(parameter => parameter.Value is not null);
}
