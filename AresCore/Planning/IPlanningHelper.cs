using Ares.Messaging;

namespace Ares.Core.Planning;

public interface IPlanningHelper
{
  /// <summary>
  /// </summary>
  /// <param name="planSuggestions">A collection of suggestions indicating which planner to pick</param>
  /// <returns>True if planning succeeded, false otherwise</returns>
  Task<bool> TryResolveParameters(IEnumerable<PlanSuggestion> planSuggestions, IEnumerable<Parameter> parameters);
}
