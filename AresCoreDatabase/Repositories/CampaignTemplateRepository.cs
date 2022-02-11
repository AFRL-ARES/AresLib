using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class CampaignTemplateRepository : RepositoryBase<CampaignTemplate>, ICampaignTemplateRepository
{
  public CampaignTemplateRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public CampaignTemplateRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
