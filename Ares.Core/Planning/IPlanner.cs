using Ares.Messaging;
using Ares.Messaging.Planning;

namespace Ares.Core.Planning;

public interface IPlanner
{
  /// <summary>
  /// Optional name for the planner (can be useful when multiple planners of same type and version have to be used)
  /// </summary>
  string Name { get; set; }

  /// <summary>
  /// Version of the planner
  /// </summary>
  Version Version { get; set; }

  /// <summary>
  /// Address used to communicate with the planner
  /// </summary>
  string Address { get; set; }

  /// <summary>
  /// Current state (<see cref="State" />) of the planner which essentially indicated
  /// whether or not this planner is currently available for planning
  /// </summary>
  IObservable<PlannerState> State { get; }

  /// <summary>
  /// A list of planners reported to be available by the planner service
  /// </summary>
  IList<Planner> AvailablePlanners { get; }

  /// <summary>
  /// A list of settings custom to this planner
  /// </summary>
  IDictionary<string, List<PlannerSetting>> PlannerSettings { get; }

  /// <summary>
  /// Id used to uniquely identify this instance of the planner
  /// </summary>
  string UniqueId { get; set; }

  /// <summary>
  /// Returns the values for the given parameter metadata
  /// </summary>
  /// <param name="plannableParameters">Collection of parameter metadata to plan for</param>
  /// <param name="experimentAnalyses">The experiment results to use as a seed for planning</param>
  /// <returns>Collection of plan <see cref="PlanResult" /> which has the metadata and the value</returns>
  Task<IEnumerable<PlanResult>> Plan(IEnumerable<ParameterMetadata> plannableParameters, IEnumerable<Analysis> experimentAnalyses, CancellationToken cancellationToken);
}
