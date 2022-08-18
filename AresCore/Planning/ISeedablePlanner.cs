namespace Ares.Core.Planning;

public interface ISeedablePlanner<in TSeedParam> : IPlanner
{
  Task Seed(TSeedParam seedParam);
}
