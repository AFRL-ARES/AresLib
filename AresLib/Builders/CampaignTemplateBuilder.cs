using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using DynamicData;

namespace AresLib.Builders
{
  internal class CampaignTemplateBuilder : TemplateBuilder<CampaignTemplate, ITemplateBuilder<ExperimentTemplate>>
  {
    public CampaignTemplateBuilder(string name) : base(name) { }

    protected override ReadOnlyObservableCollection<ITemplateBuilder<ExperimentTemplate>> DeriveSubBildersSource()
    {
      SubBuildersSource = new SourceCache<ITemplateBuilder<ExperimentTemplate>, string>(subBuilder => subBuilder.Name);
      SubBuildersSource
        .Connect()
        .Bind(out var subBuilders)
        .Subscribe();

      return subBuilders;
    }

    public override CampaignTemplate Build()
    {
      throw new NotImplementedException();
    }
  }
}
