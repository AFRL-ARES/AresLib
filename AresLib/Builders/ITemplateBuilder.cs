using Google.Protobuf;

namespace AresLib.Builders
{
  public interface ITemplateBuilder<out TTemplateMessage> : IBuilder<TTemplateMessage>
    where TTemplateMessage : IMessage

  {
    string Name { get; }
  }
}
