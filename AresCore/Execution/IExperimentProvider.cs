using Ares.Messaging;

namespace Ares.Core.Execution;

public interface IExperimentProvider
{
  ExperimentTemplate Provide(CampaignTemplate campaignTemplate);
}