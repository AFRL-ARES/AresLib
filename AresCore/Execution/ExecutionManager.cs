using System.Reactive.Linq;
using Ares.Core.Composers;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.StartConditions;
using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution;

public class ExecutionManager : IExecutionManager
{
  private readonly IExecutionReporter _executionReporter;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IStartConditionCollector _startConditionCollector;
  private CancellationTokenSource? _cancellationTokenSource;

  public ExecutionManager(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    IStartConditionCollector startConditionCollector)
  {
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _startConditionCollector = startConditionCollector;
  }

  public CampaignTemplate? CampaignTemplate { get; private set; }

  private CampaignExecutor? CampaignExecutor { get; set; }

  public void LoadTemplate(CampaignTemplate template)
  {
    CampaignTemplate = template;
    CampaignExecutor = new CampaignExecutor(_experimentComposer, _planningHelper, _executionReporter, CampaignTemplate);
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
