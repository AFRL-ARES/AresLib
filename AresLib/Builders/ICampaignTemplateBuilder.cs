using System.Collections.ObjectModel;
using Ares.Core.Messages;

namespace AresLib.Builders
{
  public interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
  {
    IExperimentTemplateBuilder AddExperimentTemplateBuilder();
    void RemoveExperimentTemplateBuilder(string experimentName);
    ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
  }
}
