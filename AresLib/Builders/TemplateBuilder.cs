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
  internal abstract class TemplateBuilder<TemplateMessage> : ITemplateBuilder<TemplateMessage>
    where TemplateMessage : IMessage
  {
    protected TemplateBuilder(string name)
    {
      Name = name;
    }
    public abstract TemplateMessage Build();

    public string Name { get; }
  }
}
