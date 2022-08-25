using DynamicData;

namespace Ares.Core.Planning;

public class PlannerManager : IPlannerManager
{
  private readonly IDictionary<string, ISourceCache<IPlanner, Version>> _plannerStore
    = new Dictionary<string, ISourceCache<IPlanner, Version>>();

  public IPlanner GetPlanner(string name, Version version)
  {
    var plannerRegistered = _plannerStore.TryGetValue(name, out var plannerVersions);
    if (!plannerRegistered)
      throw new KeyNotFoundException($"Unable to find planner {name} in the registry.");

    var planner = plannerVersions!.Lookup(version);
    if (!planner.HasValue)
      throw new KeyNotFoundException($"Unable to find planner {name} with version {version} in the registry.");

    return planner.Value;
  }

  public IPlanner GetPlanner(string name)
  {
    var plannerRegistered = _plannerStore.TryGetValue(name, out var plannerVersions);
    if (!plannerRegistered)
      throw new KeyNotFoundException($"Unable to find planner {name} in the registry.");

    return plannerVersions!.Items.OrderByDescending(planner => planner.Version).First();
  }

  public async Task RegisterPlanner(IPlanner planner)
  {
    await planner.Init();
    var plannerNameRegistered = _plannerStore.ContainsKey(planner.Name);
    if (!plannerNameRegistered)
      _plannerStore[planner.Name] = new SourceCache<IPlanner, Version>(p => p.Version);

    var plannerVersionStore = _plannerStore[planner.Name];
    plannerVersionStore.AddOrUpdate(planner);
  }
}
