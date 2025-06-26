using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.Executors.Composers;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Execution.StopConditions;
using Ares.Messaging;
using DynamicData;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Execution;

public class ExecutionManager : IExecutionManager
{
  private readonly IActiveCampaignTemplateStore _activeCampaignTemplateStore;
  private readonly ICommandComposer<CampaignTemplate, ICampaignExecutor> _campaignComposer;
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContext;
  private readonly IEnumerable<IStartCondition> _startConditions;
  private ExecutionControlTokenSource? _executionControlTokenSource;

  public ExecutionManager(IEnumerable<IStartCondition> startConditions,
    IDbContextFactory<CoreDatabaseContext> dbContext,
    IActiveCampaignTemplateStore activeCampaignTemplateStore,
    ICommandComposer<CampaignTemplate, ICampaignExecutor> campaignComposer)
  {
    _startConditions = startConditions;
    _dbContext = dbContext;
    _activeCampaignTemplateStore = activeCampaignTemplateStore;
    _campaignComposer = campaignComposer;
  }

  public IList<IStopCondition> CampaignStopConditions { get; } = new List<IStopCondition>() { };

  public async Task<bool> CanRun()
  {
    if(_activeCampaignTemplateStore.CampaignTemplate is null)
      return false;

    var startConditionTasks = _startConditions.Select(sc => sc.CanStart());
    var startConditions = await Task.WhenAll(startConditionTasks);
    return startConditions.All(condition => condition?.Success ?? true);
  }



  public int ReplanRate { get; private set; } = 1;

  public async Task Start()
  {
    var err = await CheckCampaignStartPrerequisites();
    if(!string.IsNullOrEmpty(err))
    {
      throw new InvalidOperationException(err);
    }
    var executor = _campaignComposer.Compose(_activeCampaignTemplateStore.CampaignTemplate!);
    executor.StopConditions.Add(CampaignStopConditions);
    executor.ReplanRate = ReplanRate;
    _executionControlTokenSource = new ExecutionControlTokenSource();
    var CampaignExecutionSummary = await executor.Execute(_executionControlTokenSource.Token);
    CampaignExecutionSummary.CampaignName = _activeCampaignTemplateStore.CampaignTemplate!.Name;
    PostExecution(CampaignExecutionSummary);
  }

  public void Stop()
    => _executionControlTokenSource?.Cancel();

  public void Pause()
    => _executionControlTokenSource?.Pause();

  public void Resume()
    => _executionControlTokenSource?.Resume();

  public async Task<string> CheckCampaignStartPrerequisites()
  {
    if(_activeCampaignTemplateStore.CampaignTemplate is null)
      return "CampaignTemplate was not assigned to the active template store.";

    if(!CampaignStopConditions.Any())
      return "The Campaign has no stop conditions, please set a stop condition before starting campaign.";

    var startConditionTasks = _startConditions.Select(sc => sc.CanStart());
    var startConditions = await Task.WhenAll(startConditionTasks);
    var failedStartConditions = startConditions.Where(sc => !sc.Success);
    if(failedStartConditions.Any())
      return $"Failed to start campaign:{Environment.NewLine}{string.Join(Environment.NewLine, failedStartConditions.SelectMany(conditionResult => conditionResult!.Messages))}";

    return string.Empty;
  }

  public void UpdateReplanRate(int newRate)
  {
    ReplanRate = newRate;
  }

  private void PostExecution(CampaignExecutionSummary result)
  {
    //await StoreCompletedCampaign(result);
    _executionControlTokenSource?.Dispose();
    _executionControlTokenSource = null;
  }

  private async Task StoreCompletedCampaign(CampaignExecutionSummary result)
  {
    await using var context = await _dbContext.CreateDbContextAsync();
    context.CampaignExecutionSummaries.Add(result);
    await context.SaveChangesAsync();
  }
}
