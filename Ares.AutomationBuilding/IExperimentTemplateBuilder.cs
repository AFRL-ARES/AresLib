using System.Collections.ObjectModel;
using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface IExperimentTemplateBuilder : ITemplateBuilder<ExperimentTemplate>
{
  ReadOnlyObservableCollection<IStepTemplateBuilder> StepTemplateBuilders { get; }
  IStepTemplateBuilder AddStepTemplateBuilder(string stepName, bool isParallel);
  void RemoveStepTemplateBuilder(string stepName);
}
