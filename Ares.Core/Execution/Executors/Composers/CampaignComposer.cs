using Ares.Core.Analyzing;
using Ares.Core.AresEnvironment;
using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors.Composers;

public class CampaignComposer : ICommandComposer<CampaignTemplate, ICampaignExecutor>
{
  private readonly IAnalyzerManager _analyzerManager;
  private readonly IExecutionReporter _executionReporter;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly ICommandComposer<ExperimentTemplate, StartupScriptExecutor> _startupScriptComposer;
  private readonly ICommandComposer<ExperimentTemplate, CloseoutScriptExecutor> _closeoutScriptComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IEnumerable<IResultHandler> _resultHandlers;
  private readonly AresVariableManager _variableManager;

  public CampaignComposer(IAnalyzerManager analyzerManager,
    ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    ICommandComposer<ExperimentTemplate, StartupScriptExecutor> startupScriptComposer,
    ICommandComposer<ExperimentTemplate, CloseoutScriptExecutor> closeoutScriptComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    IEnumerable<IResultHandler> resultHandlers,
    AresVariableManager variableManager)
  {
    _variableManager = variableManager;
    _analyzerManager = analyzerManager;
    _experimentComposer = experimentComposer;
    _startupScriptComposer = startupScriptComposer;
    _closeoutScriptComposer = closeoutScriptComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _resultHandlers = resultHandlers;
  }

  public ICampaignExecutor Compose(CampaignTemplate template)
    => new CampaignExecutor(_experimentComposer, _startupScriptComposer, _closeoutScriptComposer, _planningHelper, _executionReporter, _analyzerManager, template, _resultHandlers, _variableManager);
}
