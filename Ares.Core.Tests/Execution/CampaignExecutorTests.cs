using Ares.Core.Analyzing;
using Ares.Core.Device;
using Ares.Core.Execution;
using Ares.Core.Execution.ControlTokens;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.Executors.Composers;
using Ares.Core.Execution.StopConditions;
using Ares.Core.Planning;
using Ares.Core.Tests.Data;
using Ares.Core.Tests.Data.Analyzer;
using Ares.Core.Tests.Data.Device;
using Moq;

namespace Ares.Core.Tests.Execution;

internal class CampaignExecutorTests
{
  private IAnalyzerManager _analyzerManager;
  private CampaignComposer _campaignComposer;
  private ICampaignExecutor _campaignExecutor;
  private IExecutionReporter _executionReporter;
  private IExecutionReportStore _executionReportStore;
  private IPlanningHelper _planningHelper;
  private IEnumerable<IResultHandler> _resultHandlers;


  [OneTimeSetUp]
  public void OneTimeSetUp()
  {
    _analyzerManager = new AnalyzerManager(new AnalysisRepo());
    _analyzerManager.RegisterAnalyzer(new TestReplyAnalyzer());
    _executionReportStore = new ExecutionReportStore();
    _executionReporter = new ExecutionReporter(_executionReportStore);
    _planningHelper = new Mock<IPlanningHelper>().Object;
    _resultHandlers = new Mock<IEnumerable<IResultHandler>>().Object;
    var device = new TestDevice();
    var cmdInterpreter = new TestDeviceInterpreter(device);
    var repo = new DeviceCommandInterpreterRepo
    {
      cmdInterpreter
    };
    var stepComposer = new StepComposer(repo);
    var experimentComposer = new ExperimentComposer(stepComposer, _analyzerManager);
    _campaignComposer = new CampaignComposer(_analyzerManager, experimentComposer, _planningHelper, _executionReporter, _resultHandlers);
  }

  [SetUp]
  public void SetUp()
  {
    _campaignExecutor = _campaignComposer.Compose(TestCampaignProvider.GetSampleCampaignTemplate());
  }

  [Test]
  public void Executor_Should_Execute_Valid_Template_Without_Exception()
  {
    var controlTokenSource = new ExecutionControlTokenSource();
    var stopCondition = new NumExperimentsRun(_executionReportStore, 1);
    _campaignExecutor.StopConditions.Add(stopCondition);
    Assert.DoesNotThrowAsync(() => _campaignExecutor.Execute(controlTokenSource.Token));
  }
}
