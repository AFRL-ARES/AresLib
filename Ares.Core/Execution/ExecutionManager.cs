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
  private ICampaignExecutor? _currentExecutor;

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

  public bool CanRun => _startConditions.All(condition => condition.CanStart()?.Success ?? true) && _activeCampaignTemplateStore.CampaignTemplate is not null;

  public int ReplanRate { get; private set; } = 1;

  public async Task Start(string executionNotes)
  {
    CheckCampaignStartPrerequisites();
    _currentExecutor = _campaignComposer.Compose(_activeCampaignTemplateStore.CampaignTemplate!);
    if(!string.IsNullOrEmpty(executionNotes))
      _currentExecutor.UpdateExecutionNotes(executionNotes);

    _currentExecutor.StopConditions.Add(CampaignStopConditions);
    _currentExecutor.ReplanRate = ReplanRate;
    _executionControlTokenSource = new ExecutionControlTokenSource();
    var campaignResult = await _currentExecutor.Execute(_executionControlTokenSource);
    campaignResult.CampaignName = _activeCampaignTemplateStore.CampaignTemplate!.Name;
    await PostExecution(campaignResult);
  }

  public void Stop()
    => _executionControlTokenSource?.Cancel();

  public void Pause()
    => _executionControlTokenSource?.Pause();

  public void Resume()
    => _executionControlTokenSource?.Resume();

  public string CheckCampaignStartPrerequisites()
  {
    if(_activeCampaignTemplateStore.CampaignTemplate is null)
      return "CampaignTemplate was not assigned to the active template store.";

    if(!CampaignStopConditions.Any())
      return "The Campaign has no stop conditions, please set a stop condition before starting campaign.";

    if(!EnsureParameterAssignment())
      return "The campaign has errors in it's parameter assignments, please resolve these before starting your campaign.";

    var startConditionResults = _startConditions.Select(condition => condition.CanStart()).Where(result => result is not null && !result.Success).ToArray();
    if(startConditionResults.Any())
      return $"Failed to start campaign:{Environment.NewLine}{string.Join(Environment.NewLine, startConditionResults.SelectMany(conditionResult => conditionResult!.Messages))}";

    return String.Empty;
  }

  public bool EnsureParameterAssignment()
  {
    var startupCommandsInvalid = _activeCampaignTemplateStore.CampaignTemplate!.ExperimentTemplates.First().StartupStepTemplates
    .SelectMany(step => step.CommandTemplates)
    .Any(cmd => cmd.Parameters.Any(param => param.Planned && param.PlanningMetadata is null));

    if(startupCommandsInvalid)
      return false;

    var experimentCommandsInvalid = _activeCampaignTemplateStore.CampaignTemplate!.ExperimentTemplates.First().StepTemplates
    .SelectMany(step => step.CommandTemplates)
    .Any(cmd => cmd.Parameters.Any(param => param.Planned && param.PlanningMetadata is null));

    if(experimentCommandsInvalid)
      return false;

    var closeoutCommandsInvalid = _activeCampaignTemplateStore.CampaignTemplate!.ExperimentTemplates.First().CloseoutStepTemplates
      .SelectMany(step => step.CommandTemplates)
      .Any(cmd => cmd.Parameters.Any(param => param.Planned && param.PlanningMetadata is null));

    if(closeoutCommandsInvalid)
      return false;

    return true;
  }

  public void UpdateReplanRate(int newRate)
  {
    ReplanRate = newRate;
  }

  private async Task PostExecution(CampaignResult result)
  {
    //await StoreCompletedCampaign(result);
    _executionControlTokenSource?.Dispose();
    _executionControlTokenSource = null;
    _currentExecutor = null;
  }

  private async Task StoreCompletedCampaign(CampaignResult result)
  {
    await using var context = await _dbContext.CreateDbContextAsync();
    context.CampaignResults.Add(result);
    await context.SaveChangesAsync();
  }
}
