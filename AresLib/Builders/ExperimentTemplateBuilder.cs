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
  internal class ExperimentTemplateBuilder : TemplateBuilder<ExperimentTemplate, ITemplateBuilder<StepTemplate>>
  {
    public ExperimentTemplateBuilder(string name) : base(name)
    {
    }

    protected override ReadOnlyObservableCollection<ITemplateBuilder<StepTemplate>> DeriveSubBildersSource()
    {
      SubBuildersSource = new SourceCache<ITemplateBuilder<StepTemplate>, string>(subBuilder => subBuilder.Name);
      SubBuildersSource
        .Connect()
        .Bind(out var subBuilders)
        .Subscribe();

      return subBuilders;
    }

    public override ExperimentTemplate Build()
    {
      throw new NotImplementedException();
    }
  }
}
