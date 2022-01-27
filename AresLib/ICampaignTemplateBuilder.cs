using System.Collections.ObjectModel;
using Ares.Messaging;

namespace Ares.AutomationBuilding;

public interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
{
  ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
  IExperimentTemplateBuilder AddExperimentTemplateBuilder();
  void RemoveExperimentTemplateBuilder(string experimentName);
}
