using AresLib.Executors;
using Google.Protobuf;

namespace AresLib.Composers
{
  internal interface ICommandComposer<out TDbTemplate, out TExecutor>
    where TDbTemplate : IMessage
    where TExecutor : IBaseExecutor
  {
    TExecutor Compose();
    TDbTemplate Template { get; }
  }
}
