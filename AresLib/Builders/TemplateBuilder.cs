using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Google.Protobuf;

namespace AresLib.Builders
{
  internal abstract class TemplateBuilder<TemplateMessage, OwnedBuilders>
    : ITemplateBuilder<TemplateMessage>
    where TemplateMessage : IMessage
  {
    protected TemplateBuilder(string name)
    {
      Name = name;
      ManagedSubBuilders = DeriveSubBildersSource();
    }

    protected abstract ReadOnlyObservableCollection<OwnedBuilders> DeriveSubBildersSource();
    public abstract TemplateMessage Build();

    protected ISourceCache<OwnedBuilders, string> SubBuildersSource { get; set; }
    internal ReadOnlyObservableCollection<OwnedBuilders> ManagedSubBuilders { get; }
    public string Name { get; }
  }
}
