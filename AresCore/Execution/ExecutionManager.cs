using System.Reactive.Linq;
using Ares.Core.Analyzing;
using Ares.Core.Composers;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution;

public class ExecutionManager : IExecutionManager
{
  private readonly IAnalyzerManager _analyzerManager;
  private readonly IExecutionReporter _executionReporter;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IStartConditionCollector _startConditionCollector;
  private CancellationTokenSource? _cancellationTokenSource;

  public ExecutionManager(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IAnalyzerManager analyzerManager,
    IExecutionReporter executionReporter,
    IStartConditionCollector startConditionCollector)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _analyzerManager = analyzerManager;
    _executionReporter = executionReporter;
    _startConditionCollector = startConditionCollector;
  }

  public CampaignTemplate? CampaignTemplate { get; private set; }

  private CampaignExecutor? CampaignExecutor { get; set; }

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

    if (await _startConditionCollector.CanStart.Take(1) == false)
      throw new InvalidOperationException("Something is preventing this campaign from being started.");

    _cancellationTokenSource = new CancellationTokenSource();

    var campaignResult = await CampaignExecutor.Execute(_cancellationTokenSource.Token);
  }

  public void Stop()
    => _cancellationTokenSource?.Cancel();

  public void Pause()
    => throw new NotImplementedException();
}
