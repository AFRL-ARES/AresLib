using Google.Protobuf;

namespace AresLib.Builders
{
  internal interface ITemplateBuilder<TemplateMessage> : IBuilder<TemplateMessage> where TemplateMessage : IMessage
  {
    TemplateMessage Build();
  }
}
