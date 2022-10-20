using Ares.Messaging;

namespace Ares.Core.Validation.Campaign;

public interface ICampaignValidator
{
  ValidationResult Validate(CampaignTemplate template);
}
