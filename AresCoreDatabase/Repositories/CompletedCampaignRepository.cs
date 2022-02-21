using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class CompletedCampaignRepository : RepositoryBase<CompletedCampaign>, ICompletedCampaignRepository
{
  public CompletedCampaignRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public CompletedCampaignRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
