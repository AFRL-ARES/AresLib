using Ares.Messaging;

namespace Ares.Core.Execution;

public interface IExecutionManager
{
  IObservable<bool> CanStart { get; }
  Task LoadTemplate(Guid templateId);
  void Start();
  void Stop();
  void Pause();
}
