using Ares.Core.Execution;
using Ares.Messaging;
using Google.Protobuf;

namespace Ares.Core.Executors;

public interface IBaseExecutor<TResult, TTemplate>
  where TResult : IMessage
  where TTemplate : IMessage
{
  TTemplate Template { get; set; }
  IObservable<ExecutorState> State { get; }
  Task<TResult> Execute(CancellationToken cancellationToken);
}
