using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.Executors.Composers;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Execution.StopConditions;
using Ares.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Execution;

internal class ExecutionManager : IExecutionManager
{
  private readonly IActiveCampaignTemplateStore _activeCampaignTemplateStore;
  private readonly ICommandComposer<CampaignTemplate, CampaignExecutor> _campaignComposer;
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContext;
  private readonly IEnumerable<IStartCondition> _startConditions;
  private ExecutionControlTokenSource? _executionControlTokenSource;

  public ExecutionManager(IEnumerable<IStartCondition> startConditions,
    IDbContextFactory<CoreDatabaseContext> dbContext,
    IActiveCampaignTemplateStore activeCampaignTemplateStore,
    ICommandComposer<CampaignTemplate, CampaignExecutor> campaignComposer)
  {
    _startConditions = startConditions;
    _dbContext = dbContext;
    _activeCampaignTemplateStore = activeCampaignTemplateStore;
    _campaignComposer = campaignComposer;
  }

  public IList<IStopCondition> CampaignStopConditions { get; } = new List<IStopCondition>();

  public bool CanRun => !_startConditions.Any(condition => condition.CanStart()) && _activeCampaignTemplateStore.CampaignTemplate is not null;

  public async Task Start()
  {
    CheckCampaignStartPrerequisites();
    var executor = _campaignComposer.Compose(_activeCampaignTemplateStore.CampaignTemplate!);
    executor.StopConditions.Add(CampaignStopConditions);
    _executionControlTokenSource = new ExecutionControlTokenSource();
    var campaignResult = await executor.Execute(_executionControlTokenSource.Token);
    await PostExecution(campaignResult);
  }

  public void Stop()
    => _executionControlTokenSource?.Cancel();

  public void Pause()
    => _executionControlTokenSource?.Pause();

  public void Resume()
    => _executionControlTokenSource?.Resume();

  private void CheckCampaignStartPrerequisites()
  {
    if (_activeCampaignTemplateStore.CampaignTemplate is null)
      throw new InvalidOperationException("CampaignTemplate was not assigned to the active template store.");

    var failedStartConditions = _startConditions.Where(condition => !condition.CanStart()).ToArray();
    if (failedStartConditions.Any())
      throw new InvalidOperationException($"Failed to start campaign:{Environment.NewLine}{string.Join(Environment.NewLine, failedStartConditions.Select(condition => condition.Message))}");
  }

  private async Task PostExecution(CampaignResult result)
  {
    await StoreCompletedCampaign(result);
    _executionControlTokenSource?.Dispose();
    _executionControlTokenSource = null;
  }

  private async Task StoreCompletedCampaign(CampaignResult result)
  {
    await using var context = await _dbContext.CreateDbContextAsync();
    context.CampaignResults.Add(result);
    await context.SaveChangesAsync();
  }
}
