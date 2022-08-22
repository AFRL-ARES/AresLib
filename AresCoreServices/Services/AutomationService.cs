﻿using System.Threading.Tasks;
using Ares.Core.Grpc.Helpers;
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
    var projects = await dbContext.Projects.AsNoTracking().ToArrayAsync(context.CancellationToken);
    var response = new ProjectsResponse();
    response.Projects.AddRange(projects);
    return response;
  }

  public override async Task<CampaignsResponse> GetAllCampaigns(Empty request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    var campaignsResponse = new CampaignsResponse();
    var campaigns = await dbContext.CampaignTemplates.AsNoTracking().ToArrayAsync(context.CancellationToken);
    campaignsResponse.CampaignTemplates.Add(campaigns);
    return campaignsResponse;
  }

  public override async Task<BoolValue> CampaignExists(CampaignRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    await dbContext.Database.OpenConnectionAsync();
    bool exists;
    if (!string.IsNullOrEmpty(request.UniqueId))
      exists = await dbContext.CampaignTemplates.AsNoTracking().AnyAsync(template => template.UniqueId == request.UniqueId, context.CancellationToken);
    else
      exists = await dbContext.CampaignTemplates.AsNoTracking().AnyAsync(template => template.Name == request.CampaignName, context.CancellationToken);

    return new BoolValue { Value = exists };
  }

  public override async Task<Project> GetProject(ProjectRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    return await dbContext.Projects.AsNoTracking().FirstAsync(project => project.Name == request.ProjectName, context.CancellationToken);
  }

  public override Task<CampaignTemplate> GetSingleCampaign(CampaignRequest request, ServerCallContext context)
    => GetCampaignTemplate(request, context);

  public override async Task<Empty> RemoveCampaign(CampaignRequest request, ServerCallContext context)
  {
    var campaignTemplate = await GetCampaignTemplate(request, context);
    await using var dbContext = _coreContextFactory.CreateDbContext();
    dbContext.CampaignTemplates.Remove(campaignTemplate);
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

    var existingCampaign = await dbContext.CampaignTemplates.FirstAsync(template => template.UniqueId == request.UniqueId);
    dbContext.CampaignTemplates.Remove(existingCampaign);
    await dbContext.SaveChangesAsync();
    dbContext.ChangeTracker.Clear();
    request.ConsolidatePlannedParameterMetadata();
    dbContext.CampaignTemplates.Add(request);
    // existingCampaign.UpdateCampaignTemplate(request, dbContext);
    // await dbContext.SaveChangesAsync();
    // dbContext.ChangeTracker.Clear();
    // dbContext.CampaignTemplates.Update(request);
    await dbContext.SaveChangesAsync();
    // var currentTemplate = await dbContext.CampaignTemplates.FirstAsync(template => template.Name == request.CampaignName, context.CancellationToken);
    // currentTemplate.Name = request.CampaignTemplate.Name;
    // currentTemplate.PlannableParameters.Clear();
    // currentTemplate.PlannableParameters.Add(request.CampaignTemplate.PlannableParameters);
    // currentTemplate.ExperimentTemplates.Clear();
    // currentTemplate.ExperimentTemplates.Add(request.CampaignTemplate.ExperimentTemplates);
    return request;
  }

  private async Task<CampaignTemplate> GetCampaignTemplate(CampaignRequest request, ServerCallContext context)
  {
    await using var dbContext = _coreContextFactory.CreateDbContext();
    if (!string.IsNullOrEmpty(request.UniqueId))
      return await dbContext.CampaignTemplates.AsNoTracking().FirstAsync(template => template.UniqueId == request.UniqueId, context.CancellationToken);

    return await dbContext.CampaignTemplates.AsNoTracking().FirstAsync(template => template.Name == request.CampaignName);
  }
}