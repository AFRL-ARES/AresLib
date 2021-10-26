using System.Collections.ObjectModel;
using DynamicData;
using Google.Protobuf;

namespace AresLib.Builders
{
  public interface ITemplateBuilder<TemplateMessage> : IBuilder<TemplateMessage> 
    where TemplateMessage : IMessage

  {
    string Name { get; }
  }
}
