using Ardalis.Specification;
using Ares.Messaging;

namespace Ares.Core.EFCore;

public sealed class CampaignTemplateByNameSpecification : Specification<CampaignTemplate>, ISingleResultSpecification<CampaignTemplate>
{
  public CampaignTemplateByNameSpecification(string templateName)
  {
    Query.Where(template => template.Name == templateName);
  }
}
