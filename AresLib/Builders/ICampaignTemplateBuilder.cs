using Ares.Core;
using System.Collections.ObjectModel;

namespace AresLib.Builders
{
  public interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
  {
    IExperimentTemplateBuilder AddExperimentTemplateBuilder();
    void RemoveExperimentTemplateBuilder(string experimentName);
    ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
  }
}
