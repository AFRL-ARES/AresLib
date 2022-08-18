using Google.Protobuf;

namespace Ares.Core.Composers;

internal interface ICommandComposer<out TDbTemplate, out TExecutor>
  where TDbTemplate : IMessage
{
  TDbTemplate Template { get; }
  TExecutor Compose();
}
