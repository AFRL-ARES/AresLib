﻿using Ares.Core.Analyzing;
using Ares.Core.Planning;
using Ares.Messaging;

namespace Ares.Core.Execution.Executors.Composers;

public class CampaignComposer : ICommandComposer<CampaignTemplate, ICampaignExecutor>
{
  private readonly IAnalyzerManager _analyzerManager;
  private readonly IExecutionReporter _executionReporter;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;
  private readonly IEnumerable<IResultHandler> _resultHandlers;

  public CampaignComposer(IAnalyzerManager analyzerManager,
    ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper,
    IExecutionReporter executionReporter,
    IEnumerable<IResultHandler> resultHandlers)
  {
    _analyzerManager = analyzerManager;
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    _executionReporter = executionReporter;
    _resultHandlers = resultHandlers;
  }

  public ICampaignExecutor Compose(CampaignTemplate template) => new CampaignExecutor(_experimentComposer, _planningHelper, _executionReporter, _analyzerManager, template, _resultHandlers);
}
