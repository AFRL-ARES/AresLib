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

  public static ExperimentTemplate CloneWithNewIds(this ExperimentTemplate template)
  {
    var newTemplate = template.Clone();
    newTemplate.UniqueId = Guid.NewGuid().ToString();
    foreach (var stepTemplate in newTemplate.StepTemplates)
    {
      stepTemplate.UniqueId = Guid.NewGuid().ToString();
      foreach (var commandTemplate in stepTemplate.CommandTemplates)
      {
        commandTemplate.UniqueId = Guid.NewGuid().ToString();
        foreach (var argument in commandTemplate.Arguments)
        {
          argument.UniqueId = Guid.NewGuid().ToString();
          argument.Metadata.UniqueId = Guid.NewGuid().ToString();
          if (argument.Value is not null)
            argument.Value.UniqueId = Guid.NewGuid().ToString();
          foreach (var constraint in argument.Metadata.Constraints)
          {
            constraint.UniqueId = Guid.NewGuid().ToString();
          }
        }
      }
    }

    return newTemplate;
  }
}
