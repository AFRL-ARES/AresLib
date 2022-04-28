using System.Threading.Tasks;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Grpc.Services;

public class AutomationService : AresAutomation.AresAutomationBase
{
  private readonly IDbContextFactory<CoreDatabaseContext> _coreContextFactory;

  public AutomationService(IDbContextFactory<CoreDatabaseContext> coreContextFactory)
  {
    _coreContextFactory = coreContextFactory;
  }


  public override async Task<ProjectsResponse> GetAllProjects(Empty request, ServerCallContext context)
  {
    await using var dbContext = await _coreContextFactory.CreateDbContextAsync();
    var projects = await dbContext.Projects.ToArrayAsync(context.CancellationToken);
    var response = new ProjectsResponse();
    response.Projects.AddRange(projects);
    return response;
  }

  public override async Task<CampaignsResponse> GetAllCampaigns(Empty request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var campaignsResponse = new CampaignsResponse();
    var campaigns = await dbContext.CampaignTemplates.ToArrayAsync(context.CancellationToken);
    campaignsResponse.CampaignTemplates.Add(campaigns);
    return campaignsResponse;
  }

  public override async Task<BoolValue> CampaignExists(CampaignRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    await dbContext.Database.OpenConnectionAsync();
    var exists = await dbContext.CampaignTemplates.AnyAsync(template => template.Name == request.CampaignName, context.CancellationToken);
    return new BoolValue { Value = exists };
  }

  public override async Task<Project> GetProject(ProjectRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    return await dbContext.Projects.FirstAsync(project => project.Name == request.ProjectName, context.CancellationToken);
  }

  public override async Task<CampaignTemplate> GetSingleCampaign(CampaignRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    return await dbContext.CampaignTemplates.FirstAsync(template => template.Name == request.CampaignName);
  }

  public override async Task<Empty> RemoveCampaign(CampaignRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var campaignTemplate = dbContext.CampaignTemplates.Remove(await dbContext.CampaignTemplates.FirstAsync(template => template.Name == request.CampaignName));
    await dbContext.SaveChangesAsync(context.CancellationToken);

    return new Empty();
  }

  public override async Task<Empty> RemoveProject(ProjectRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var project = await dbContext.Projects.FirstAsync(p => p.Name == request.ProjectName, context.CancellationToken);
    dbContext.Projects.Remove(project);
    await dbContext.SaveChangesAsync(context.CancellationToken);

    return new Empty();
  }

  public override async Task<Empty> AddProject(Project request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.Projects.Add(request);
    await dbContext.SaveChangesAsync(context.CancellationToken);
    return new Empty();
  }

  public override async Task<Empty> AddCampaign(CampaignTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CampaignTemplates.Add(request);
    await dbContext.SaveChangesAsync(context.CancellationToken);
    return new Empty();
  }

  public override async Task<Empty> UpdateCampaign(CampaignUpdateRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var currentTemplate = await dbContext.CampaignTemplates.FirstAsync(template => template.Name == request.CampaignName, context.CancellationToken);
    currentTemplate.Name = request.CampaignTemplate.Name;
    currentTemplate.PlannableParameters.Clear();
    currentTemplate.PlannableParameters.Add(request.CampaignTemplate.PlannableParameters);
    currentTemplate.ExperimentTemplates.Clear();
    currentTemplate.ExperimentTemplates.Add(request.CampaignTemplate.ExperimentTemplates);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }
}
