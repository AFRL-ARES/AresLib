using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;

namespace AresLib.Builders
{
  public interface ICampaignTemplateBuilder : ITemplateBuilder<CampaignTemplate>
  {
    IExperimentTemplateBuilder AddExperimentTemplateBuilder();
    void RemoveExperimentTemplateBuilder(string experimentName);
    ReadOnlyObservableCollection<IExperimentTemplateBuilder> ExperimentTemplateBuilders { get; }
  }
}
