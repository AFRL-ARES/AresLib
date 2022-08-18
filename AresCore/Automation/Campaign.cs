using Ares.Messaging;

namespace Ares.Core.Automation;

internal class Campaign
{
  public State State = State.NotStarted;
  public string Name = string.Empty;
  public static Campaign FromTemplate(CampaignTemplate template)
  {
    var campaign = new Campaign();
    campaign.Name = template.Name;
    return campaign;
  }
  
  
}
