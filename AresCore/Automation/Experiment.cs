using Ares.Messaging;

namespace Ares.Core.Automation;

internal class Experiment<TResult>
{
  public string Name = string.Empty;
  
  public static Experiment<TResult> FromTemplate(ExperimentTemplate template)
  {
    var experiment = new Experiment<TResult>();
    experiment.Name = template.Name;
    return experiment;
  }
}
