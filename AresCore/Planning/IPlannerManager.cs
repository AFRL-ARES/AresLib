using DynamicData;

namespace Ares.Core.Planning;

public interface IPlannerManager
{
  T GetPlanner<T>() where T : IPlanner;
  T GetPlanner<T>(Version version) where T : IPlanner;
  T GetPlanner<T>(string name, Version version) where T : IPlanner;
  IPlanner GetPlanner(string type);
  IPlanner GetPlanner(string type, Version version);
  IPlanner GetPlanner(string type, string name, Version version);
  IPlanner GetPlanner(string type, string name);
  void RegisterPlanner(IPlanner planner);
  IEnumerable<IPlanner> AvailablePlanners { get; }
}