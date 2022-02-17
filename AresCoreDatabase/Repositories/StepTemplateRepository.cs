using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class StepTemplateRepository : RepositoryBase<StepTemplate>, IStepTemplateRepository
{
  public StepTemplateRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public StepTemplateRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
