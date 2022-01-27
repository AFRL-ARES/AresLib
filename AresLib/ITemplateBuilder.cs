using Google.Protobuf;

namespace Ares.AutomationBuilding;

public interface ITemplateBuilder<out TTemplateMessage> : IBuilder<TTemplateMessage>
  where TTemplateMessage : IMessage

{
  string Name { get; }
}
