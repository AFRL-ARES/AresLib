using Ares.Messaging;

namespace Ares.Core.Execution;
public interface IResultHandler
{
  Task Handle(ExperimentResult result);
}
