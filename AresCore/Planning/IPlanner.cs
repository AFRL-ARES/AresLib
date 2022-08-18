using Ares.Messaging;

namespace Ares.Core.Planning;

public interface IPlanner
{
  string Name { get; }
  Version Version { get; }
  Task<IEnumerable<PlanResult>> Plan(IEnumerable<ParameterMetadata> plannableParameters);
  void Reset();
  Task Init();
}
