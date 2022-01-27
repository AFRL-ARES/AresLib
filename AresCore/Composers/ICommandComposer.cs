using Ares.Core.Executors;
using Google.Protobuf;

namespace Ares.Core.Composers;

internal interface ICommandComposer<out TDbTemplate, out TExecutor>
  where TDbTemplate : IMessage
  where TExecutor : IBaseExecutor
{
  TDbTemplate Template { get; }
  TExecutor Compose();
}
