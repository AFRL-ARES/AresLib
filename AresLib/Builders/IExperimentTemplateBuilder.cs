using Ares.Core;
using System.Collections.ObjectModel;

namespace AresLib.Builders
{
  public interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
  {
    IStepTemplateBuilder AddStepTemplateBuilder(string stepName, bool isParallel);
    void RemoveStepTemplateBuilder(string stepName);
    ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
  }
}
