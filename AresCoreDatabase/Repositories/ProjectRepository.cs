using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Ares.Core.Repositories;
using Ares.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.EFCore.Repositories;

public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
  public ProjectRepository(DbContext dbContext) : base(dbContext)
  {
  }

  public ProjectRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
  {
  }
}
