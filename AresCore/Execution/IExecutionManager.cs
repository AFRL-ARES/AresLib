namespace Ares.Core.Execution;

public interface IExecutionManager
{
  Task LoadTemplate(Guid templateId);
}
