using Ardalis.Specification;
using Ares.Messaging;

namespace Ares.Core.Repositories.Specifications;

public sealed class CompletedCampaignByNameSpecification : Specification<CompletedCampaign>, ISingleResultSpecification<CompletedCampaign>
{
  public CompletedCampaignByNameSpecification(string name)
  {
    Query.Where(campaign => campaign.Template.Name == name);
  }
}
