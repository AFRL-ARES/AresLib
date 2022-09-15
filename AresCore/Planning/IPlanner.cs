using System.Data;
using Ares.Messaging;

namespace Ares.Core.Planning;

public interface IPlanner
{
  string Name { get; }
  Version Version { get; }
  Task<IEnumerable<PlanResult>> Plan(IEnumerable<ParameterMetadata> plannableParameters);
  IObservable<PlannerState> PlannerState { get; }
  
  void Reset();
  Task Init();
}
