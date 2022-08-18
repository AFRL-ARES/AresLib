using DynamicData;

namespace Ares.Core.Planning;

public interface IPlannerManager
{
  IPlanner GetPlanner(string name, Version version);
  IPlanner GetPlanner(string name);
  Task RegisterPlanner(IPlanner planner);
}