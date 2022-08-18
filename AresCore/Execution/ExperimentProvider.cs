using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution;

class ExperimentProvider : IExperimentProvider
{
  public ExperimentProvider(IPlanner planner)
  {
    
  }
  
  public ExperimentTemplate Provide(CampaignTemplate campaignTemplate)
    => throw new NotImplementedException();
}
