using Ares.Core.Composers;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

internal class CampaignExecutor
{
  private readonly CancellationTokenSource _cancellationTokenSource;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly CampaignTemplate _template;

  public CampaignExecutor(ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer, CampaignTemplate template)
  {
    _experimentComposer = experimentComposer;
    _template = template;
    _cancellationTokenSource = new CancellationTokenSource();
  }

  public async Task<CampaignResult> Execute(CancellationToken cancellationToken)
  {
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();
    while (!ShouldStop())
    {
      var experimentExecutor = GenerateExperimentExecutor();
      var experimentResult = await experimentExecutor.Execute(_cancellationTokenSource.Token);
      experimentResults.Add(experimentResult);
    }

    var campaignResult = new CampaignResult
    {
      CampaignId = _template.UniqueId,
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = DateTime.UtcNow.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      }
    };

    campaignResult.ExperimentResults.AddRange(experimentResults);

    return campaignResult;
  }

  private bool ShouldStop()
    => false;

  private ExperimentExecutor GenerateExperimentExecutor()
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = _template.ExperimentTemplates.First();
    return _experimentComposer.Compose(experimentTemplate);
  }
}
