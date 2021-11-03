using System.Collections.ObjectModel;
using Ares.Core.Messages;

namespace AresLib.Builders
{
  public interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
  {
    IStepTemplateBuilder AddStepTemplateBuilder(string stepName, bool isParallel);
    void RemoveStepTemplateBuilder(string stepName);
    ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
  }
}
