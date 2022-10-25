using Google.Protobuf;

namespace Ares.AutomationBuilding;

public abstract class TemplateBuilder<TTemplateMessage> : ITemplateBuilder<TTemplateMessage>
  where TTemplateMessage : IMessage
{
  protected TemplateBuilder(string name)
  {
    Name = name;
  }

  public abstract TTemplateMessage Build();

  public string Name { get; }
}