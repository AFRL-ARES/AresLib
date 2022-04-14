using Ardalis.Specification;
using Ares.Messaging;

namespace Ares.Core.Repositories.Specifications;

public sealed class ProjectByNameSpecification : Specification<Project>, ISingleResultSpecification<Project>
{
  public ProjectByNameSpecification(string name)
  {
    Query.Where(project => project.Name == name);
  }
}
