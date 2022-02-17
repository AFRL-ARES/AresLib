using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class PlannerTransactionRepository : RepositoryBase<PlannerTransaction>, IPlannerTransactionRepository
{
  public PlannerTransactionRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public PlannerTransactionRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
