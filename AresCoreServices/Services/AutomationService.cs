using System.Threading.Tasks;
using Ares.Core.Repositories;
using Ares.Core.Repositories.Specifications;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc.Services;

public class AutomationService : AresAutomation.AresAutomationBase
{
  private readonly ICampaignTemplateRepository _campaignTemplateRepository;
  private readonly IProjectRepository _projectRepository;

  public AutomationService(ICampaignTemplateRepository campaignTemplateRepository, IProjectRepository projectRepository)
  {
    _campaignTemplateRepository = campaignTemplateRepository;
    _projectRepository = projectRepository;
  }


  public override async Task<ProjectsResponse> GetAllProjects(Empty request, ServerCallContext context)
  {
    var projects = await _projectRepository.ListAsync();
    var response = new ProjectsResponse();
    response.Projects.AddRange(projects);
    return response;
  }

  public override async Task<CampaignsResponse> GetAllCampaigns(Empty request, ServerCallContext context)
  {
    var campaignsResponse = new CampaignsResponse();
    var campaigns = await _campaignTemplateRepository.ListAsync(context.CancellationToken);
    campaignsResponse.CampaignTemplates.Add(campaigns);
    return campaignsResponse;
  }

  public override Task<Project> GetProject(ProjectRequest request, ServerCallContext context)
  {
    return _projectRepository.GetBySpecAsync(new ProjectByNameSpecification(request.ProjectName), context.CancellationToken);
  }

  public override Task<CampaignTemplate> GetSingleCampaign(CampaignRequest request, ServerCallContext context)
  {
    return _campaignTemplateRepository.GetBySpecAsync(new CampaignTemplateByNameSpecification(request.CampaignName), context.CancellationToken);
  }

  public override async Task<Empty> RemoveCampaign(CampaignRequest request, ServerCallContext context)
  {
    var campaignTemplate = await _campaignTemplateRepository.GetBySpecAsync(new CampaignTemplateByNameSpecification(request.CampaignName), context.CancellationToken);
    if (campaignTemplate is not null)
      await _campaignTemplateRepository.DeleteAsync(campaignTemplate, context.CancellationToken);

    return new Empty();
  }

  public override async Task<Empty> RemoveProject(ProjectRequest request, ServerCallContext context)
  {
    var project = await _projectRepository.GetBySpecAsync(new ProjectByNameSpecification(request.ProjectName), context.CancellationToken);
    if (project is not null)
      await _projectRepository.DeleteAsync(project, context.CancellationToken);

    return new Empty();
  }

  public override async Task<Empty> AddProject(Project request, ServerCallContext context)
  {
    await _projectRepository.AddAsync(request, context.CancellationToken);
    return new Empty();
  }

  public override async Task<Empty> AddCampaign(CampaignTemplate request, ServerCallContext context)
  {
    await _campaignTemplateRepository.AddAsync(request, context.CancellationToken);
    return new Empty();
  }
}
