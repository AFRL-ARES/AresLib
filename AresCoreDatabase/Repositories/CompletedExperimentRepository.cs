using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class CompletedExperimentRepository : RepositoryBase<CompletedExperiment>, ICompletedExperimentRepository
{
  public CompletedExperimentRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public CompletedExperimentRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
