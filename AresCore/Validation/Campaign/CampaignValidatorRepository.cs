using System.Collections.ObjectModel;
using DynamicData;

namespace Ares.Core.Validation.Campaign;

internal class CampaignValidatorRepository : Collection<ICampaignValidator>, ICampaignValidatorRepository
{
  public CampaignValidatorRepository(IEnumerable<ICampaignValidator> campaignValidators)
  {
    this.AddRange(campaignValidators);
  }
}
