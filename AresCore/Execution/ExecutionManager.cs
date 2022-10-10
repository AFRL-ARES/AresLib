﻿using Ares.Core.Analyzing;
using Ares.Core.Composers;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Execution.StopConditions;
using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution;

public class ExecutionManager : IExecutionManager
{
  private readonly IAnalyzerManager _analyzerManager;
  private readonly IExecutionReporter _executionReporter;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IStartConditionRegistry _startConditionRegistry;
  private CancellationTokenSource? _campaignCancellationTokenSource;
  private PauseTokenSource? _campaignPauseTokenSource;

  public ExecutionManager(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IAnalyzerManager analyzerManager,
    IExecutionReporter executionReporter,
    IStartConditionRegistry startConditionRegistry)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _analyzerManager = analyzerManager;
    _executionReporter = executionReporter;
    _startConditionRegistry = startConditionRegistry;
  }

  public CampaignTemplate? CampaignTemplate { get; private set; }

  private CampaignExecutor? CampaignExecutor { get; set; }

  public IList<IStopCondition>? CampaignStopConditions => CampaignExecutor?.StopConditions;

  public bool CanRun => !_startConditionRegistry.GetFailedConditions().Any() && CampaignExecutor is not null;

  public void LoadTemplate(CampaignTemplate template)
  {
    CampaignTemplate = template;
    var analyzer = template.Analyzer is null ? _analyzerManager.GetAnalyzer<NoneAnalyzer>() : _analyzerManager.GetAnalyzer(template.Analyzer.Type, template.Analyzer.Version);
    CampaignExecutor = new CampaignExecutor(_experimentComposer, _planningHelper, _executionReporter, analyzer, CampaignTemplate);
  }

  public async void Start()
  {
    if (CampaignExecutor is null)
      throw new InvalidOperationException("Campaign template has not been set");

    var failedStartConditions = _startConditionRegistry.GetFailedConditions().ToArray();
    if (failedStartConditions.Any())
      throw new InvalidOperationException($"Failed to start campaign:{Environment.NewLine}{string.Join(Environment.NewLine, failedStartConditions.Select(condition => condition.Message))}");

    _campaignCancellationTokenSource = new CancellationTokenSource();
    _campaignPauseTokenSource = new PauseTokenSource();

    var campaignResult = await CampaignExecutor.Execute(_campaignCancellationTokenSource.Token, _campaignPauseTokenSource.Token);

    _campaignPauseTokenSource.Dispose();
    _campaignCancellationTokenSource.Dispose();
    _campaignCancellationTokenSource = null;
    _campaignPauseTokenSource = null;
  }

  public void Stop()
    => _campaignCancellationTokenSource?.Cancel();

  public void Pause()
    => _campaignPauseTokenSource?.Pause();

  public void Resume()
    => _campaignPauseTokenSource?.Resume();
}
