using System.Linq;
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

  /// <summary>
  /// </summary>
  /// <param name="request">
  ///   <see
  /// </param>
  /// <param name="context"></param>
  /// <returns></returns>
  public override async Task<Empty> AddCampaign(CampaignTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CampaignTemplates.Add(request);
    await dbContext.SaveChangesAsync(context.CancellationToken);
    return new Empty();
  }

  public override async Task<CampaignTemplate> UpdateCampaign(CampaignTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CampaignTemplates.Update(request);
    // var currentTemplate = await dbContext.CampaignTemplates.FirstAsync(template => template.Name == request.CampaignName, context.CancellationToken);
    // currentTemplate.Name = request.CampaignTemplate.Name;
    // currentTemplate.PlannableParameters.Clear();
    // currentTemplate.PlannableParameters.Add(request.CampaignTemplate.PlannableParameters);
    // currentTemplate.ExperimentTemplates.Clear();
    // currentTemplate.ExperimentTemplates.Add(request.CampaignTemplate.ExperimentTemplates);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<ExperimentsResponse> GetExperiments(RequestById request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var experiments = await dbContext.CampaignTemplates.Where(template => template.UniqueId == request.UniqueId).SelectMany(template => template.ExperimentTemplates).ToArrayAsync();
    var experimentsResponse = new ExperimentsResponse();
    experimentsResponse.Experiments.AddRange(experiments);
    return experimentsResponse;
  }

  public override async Task<ExperimentTemplate> AddExperiment(AddExperimentRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var campaign = await dbContext.CampaignTemplates.FirstAsync(template => template.UniqueId == request.CampaignId);
    campaign.ExperimentTemplates.Add(request.ExperimentTemplate);
    await dbContext.SaveChangesAsync();
    return request.ExperimentTemplate;
  }

  public override async Task<Empty> RemoveExperiment(ExperimentTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.ExperimentTemplates.Remove(request);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }

  public override async Task<StepsResponse> GetSteps(RequestById request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var steps = await dbContext.ExperimentTemplates.Where(template => template.UniqueId == request.UniqueId).SelectMany(template => template.StepTemplates).ToArrayAsync();
    var response = new StepsResponse();
    response.Steps.AddRange(steps);
    return response;
  }

  public override async Task<ExperimentTemplate> UpdateExperiment(ExperimentTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.ExperimentTemplates.Update(request);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<StepTemplate> AddStep(AddStepRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var existingExperiment = await dbContext.ExperimentTemplates.FirstAsync(template => template.UniqueId == request.ExperimentId);
    existingExperiment.StepTemplates.Add(request.StepTemplate);
    await dbContext.SaveChangesAsync();
    return request.StepTemplate;
  }

  public override async Task<StepTemplate> UpdateStep(StepTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.StepTemplates.Update(request);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<Empty> RemoveStep(StepTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.StepTemplates.Remove(request);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }

  public override async Task<CommandsResponse> GetCommands(RequestById request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var commands = await dbContext.StepTemplates.Where(template => template.UniqueId == request.UniqueId).SelectMany(template => template.CommandTemplates).ToArrayAsync();
    var response = new CommandsResponse();
    response.Commands.AddRange(commands);
    return response;
  }

  public override async Task<CommandTemplate> AddCommand(AddCommandRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var existingStep = await dbContext.StepTemplates.FirstAsync(template => template.UniqueId == request.StepId);
    existingStep.CommandTemplates.Add(request.CommandTemplate);
    await dbContext.SaveChangesAsync();
    return request.CommandTemplate;
  }

  public override async Task<CommandTemplate> UpdateCommand(CommandTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CommandTemplates.Update(request);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<Empty> RemoveCommand(CommandTemplate request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CommandTemplates.Remove(request);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }

  public override async Task<ArgumentsResponse> GetArguments(RequestById request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var arguments = await dbContext.CommandTemplates.Where(template => template.UniqueId == request.UniqueId).SelectMany(template => template.Arguments).ToArrayAsync();
    var response = new ArgumentsResponse();
    response.Arguments.AddRange(arguments);
    return response;
  }

  public override async Task<Parameter> AddArgument(AddArgumentRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var existingCommand = await dbContext.CommandTemplates.FirstAsync(template => template.UniqueId == request.CommandId);
    existingCommand.Arguments.Add(request.Argument);
    await dbContext.SaveChangesAsync();
    return request.Argument;
  }

  public override async Task<Parameter> UpdateArgument(Parameter request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.Update(request);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<Empty> RemoveArgument(Parameter request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.Remove(request);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }

  public override async Task<CampaignParametersResponse> GetCampaignParameters(RequestById request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var parameters = await dbContext.CampaignTemplates.Where(template => template.UniqueId == request.UniqueId).SelectMany(template => template.PlannableParameters).ToArrayAsync();
    var response = new CampaignParametersResponse();
    response.Parameters.AddRange(parameters);
    return response;
  }

  public override async Task<ParameterMetadata> AddCampaignParameter(AddCampaignParametersRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var campaign = await dbContext.CampaignTemplates.FirstAsync(template => template.UniqueId == request.CampaignId);
    campaign.PlannableParameters.Add(request.Parameter);
    await dbContext.SaveChangesAsync();
    return request.Parameter;
  }

  public override async Task<ParameterMetadata> UpdateCampaignParameter(ParameterMetadata request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.Update(request);
    await dbContext.SaveChangesAsync();
    return request;
  }

  public override async Task<Empty> RemoveCampaignParameter(ParameterMetadata request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.Remove(request);
    await dbContext.SaveChangesAsync();
    return new Empty();
  }
}
