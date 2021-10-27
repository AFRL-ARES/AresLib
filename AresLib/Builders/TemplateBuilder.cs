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
