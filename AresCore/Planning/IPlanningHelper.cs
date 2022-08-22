using Ares.Messaging;

namespace Ares.Core.Planning;

public interface IPlanningHelper
{
  /// <summary>
  /// </summary>
  /// <param name="plannerAllocations">A collection of planner-to-parameter allocations indicating which planner to pick</param>
  /// <param name="parameters">Collection of parameters whose values to plan for</param>
  /// <returns>True if planning succeeded, false otherwise</returns>
  Task<bool> TryResolveParameters(IEnumerable<PlannerAllocation> plannerAllocations, IEnumerable<Parameter> parameters);
}
