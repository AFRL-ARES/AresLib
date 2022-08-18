using Ares.Core.Composers;
using Ares.Core.Execution.Executors;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Execution;

internal class ExecutionManager : IExecutionManager
{
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;
  private readonly IExperimentComposer _experimentComposer;

  public ExecutionManager(IDbContextFactory<CoreDatabaseContext> dbContextFactory, IExperimentComposer experimentComposer)
  {
    _dbContextFactory = dbContextFactory;
    _experimentComposer = experimentComposer;
  }

  public async Task LoadTemplate(Guid templateId)
  {
    using var context = _dbContextFactory.CreateDbContext();
    var template = await context.CampaignTemplates.FirstOrDefaultAsync(campaignTemplate => campaignTemplate.UniqueId == templateId.ToString());
    if (template is null)
      throw new InvalidOperationException($"Cannot find a campaign template with id {templateId}");

    CampaignExecutor = new CampaignExecutor(_experimentComposer, template);
  }

  public CampaignExecutor CampaignExecutor { get; private set; }
}
