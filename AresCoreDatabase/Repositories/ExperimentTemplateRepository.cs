using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class ExperimentTemplateRepository : RepositoryBase<ExperimentTemplate>, IExperimentTemplateRepository
{
  public ExperimentTemplateRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public ExperimentTemplateRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
