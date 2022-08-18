using Ares.Messaging;

namespace Ares.Core.Planning;

internal class PlanningHelper : IPlanningHelper
{
  private readonly IPlannerManager _plannerManager;

  public PlanningHelper(IPlannerManager plannerManager)
  {
    _plannerManager = plannerManager;
  }

  public async Task<bool> TryResolveParameters(IEnumerable<PlanSuggestion> planSuggestions, IEnumerable<Parameter> parameters)
  {
    var parameterArray = parameters.ToArray();
    var plannerSet = new Dictionary<IPlanner, ParameterMetadata>();
    foreach (var planSuggestion in planSuggestions)
    {
      var hasVersion = Version.TryParse(planSuggestion.Planner.Version, out var version);
      var planner = hasVersion
        ? _plannerManager.GetPlanner(planSuggestion.Planner.Name, version!)
        : _plannerManager.GetPlanner(planSuggestion.Planner.Name);

      plannerSet[planner] = planSuggestion.Parameter;
    }

    var planGroup = plannerSet.GroupBy(pair => pair.Key);
    foreach (var grouping in planGroup)
    {
      var planner = grouping.Key;
      var resultsEnumerable = await planner.Plan(grouping.Select(pair => pair.Value));
      var results = resultsEnumerable.ToArray();
      if (!results.Any())
        return false;

      foreach (var result in results)
      {
        var parameterPlanTarget = parameterArray.First(parameter => parameter.Metadata.UniqueId == result.Metadata.UniqueId);
        parameterPlanTarget.Value = Convert.ToSingle(result.Value);
      }
    }

    return true;
  }
}
