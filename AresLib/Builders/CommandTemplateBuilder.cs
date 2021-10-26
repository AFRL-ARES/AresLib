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
  internal class CommandTemplateBuilder : TemplateBuilder<CommandTemplate, ITemplateBuilder<CommandParameter>>
  {
    public CommandTemplateBuilder(string name) : base(name)
    {
    }

    protected override ReadOnlyObservableCollection<ITemplateBuilder<CommandParameter>> DeriveSubBildersSource()
    {
      SubBuildersSource = new SourceCache<ITemplateBuilder<CommandParameter>, string>(subBuilder => subBuilder.Name);
      SubBuildersSource
        .Connect()
        .Bind(out var subBuilders)
        .Subscribe();

      return subBuilders;
    }

    public override CommandTemplate Build()
    {
      throw new NotImplementedException();
    }
  }
}
