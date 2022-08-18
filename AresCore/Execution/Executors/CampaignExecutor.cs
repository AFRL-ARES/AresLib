using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Composers;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;

namespace Ares.Core.Execution.Executors;

public class CampaignExecutor
{
  private readonly IExperimentComposer _experimentComposer;
  private readonly CampaignTemplate _template;
  private readonly CancellationTokenSource _cancellationTokenSource;
  
  public CampaignExecutor(IExperimentComposer experimentComposer, CampaignTemplate template)
  {
    _experimentComposer = experimentComposer;
    _template = template;
    _cancellationTokenSource = new CancellationTokenSource();
  }

  public async Task<CampaignResult> Execute()
  {
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();
    while (!ShouldStop())
    {
      var experimentExecutor = GenerateExperimentExecutor(_template);
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
  
  private ExperimentExecutor GenerateExperimentExecutor(CampaignTemplate template)
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = template.ExperimentTemplates.First();
    return template.PlannableParameters.Any()
      ? _experimentComposer.Compose(experimentTemplate, template.PlannableParameters)
      : _experimentComposer.Compose(experimentTemplate);
  }

  private ExperimentTemplate ResolveTemplate(ExperimentTemplate template, IEnumerable<Parameter> resolvedArguments)
  {
    var templateCopy = CopyTemplate(template);
    var parametersToPlan = templateCopy.StepTemplates
      .SelectMany(stepTemplate => stepTemplate.CommandTemplates)
      .SelectMany(commandTemplate => commandTemplate.Arguments)
      .Where(argument => argument.Planned);

    var resolvedParametersEnumerated = resolvedArguments.ToArray();
    foreach (var unresolvedParameter in parametersToPlan)
    {
      var resolvedParameter = resolvedParametersEnumerated.First(resolvedParameter => resolvedParameter.Metadata.Name == unresolvedParameter.Metadata.Name);
      unresolvedParameter.Planned = false;
      unresolvedParameter.Value = resolvedParameter.Value;
    }

    return templateCopy;
  }

  /// <summary>
  /// Copies an existing experiment template but with a new unique ID
  /// </summary>
  /// <param name="template"></param>
  /// <returns></returns>
  private ExperimentTemplate CopyTemplate(ExperimentTemplate template)
  {
    var templateCopy = template.Clone();
    templateCopy.UniqueId = Guid.NewGuid().ToString();
    return templateCopy;
  }
}
