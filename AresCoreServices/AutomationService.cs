using System.Threading.Tasks;
using Ares.Core.Repositories;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Ares.Core.Grpc;

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
    var stuff = await _projectRepository.ListAsync();
    var response = new ProjectsResponse();
    response.Projects.AddRange(stuff);
    return response;
  }

  public override Task<CommandResult> AddCampaign(CampaignTemplate request, ServerCallContext context)
  {
    return base.AddCampaign(request, context);
  }
}
