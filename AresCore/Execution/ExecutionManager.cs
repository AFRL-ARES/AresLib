using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Ares.Core.Composers;
using Ares.Core.Execution.Executors;
using Ares.Core.Execution.Extensions;
using Ares.Core.Planning;
using Ares.Messaging;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace Ares.Core.Execution;

internal class ExecutionManager : IExecutionManager
{
  private CancellationTokenSource? _cancellationTokenSource;
  private readonly ISubject<bool> _canStartSubject = new BehaviorSubject<bool>(false);
  private readonly IDbContextFactory<CoreDatabaseContext> _dbContextFactory;
  private readonly ICommandComposer<ExperimentTemplate, ExperimentExecutor> _experimentComposer;
  private readonly IPlanningHelper _planningHelper;

  public ExecutionManager(IDbContextFactory<CoreDatabaseContext> dbContextFactory, 
    ICommandComposer<ExperimentTemplate, ExperimentExecutor> experimentComposer,
    IPlanningHelper planningHelper)
  {
    _dbContextFactory = dbContextFactory;
    _experimentComposer = experimentComposer;
    _planningHelper = planningHelper;
    CanStart = _canStartSubject.AsObservable();
  }

  public CampaignTemplate? CampaignTemplate { get; private set; }

  public async Task LoadTemplate(Guid templateId)
  {
    await using var context = _dbContextFactory.CreateDbContext();
    var template = await context.CampaignTemplates
      .AsNoTracking()
      .FirstOrDefaultAsync(campaignTemplate => campaignTemplate.UniqueId == templateId.ToString());
    if (template is null)
      Trace.WriteLine($"Cannot find a campaign template with id {templateId}");

    CampaignTemplate = template;
  }

  public async void Start()
  {
    if (CampaignTemplate is null)
      throw new InvalidOperationException("Campaign template has not been set");

    _cancellationTokenSource = new CancellationTokenSource();
    var startTime = DateTime.UtcNow;
    var experimentResults = new List<ExperimentResult>();

    var experimentExecutor = await GenerateExperimentExecutor(CampaignTemplate);
    if (experimentExecutor is not null)
    {
      var experimentResult = await experimentExecutor.Execute(_cancellationTokenSource.Token);
      experimentResults.Add(experimentResult);
    }


    var campaignResult = new CampaignResult
    {
      CampaignId = CampaignTemplate.UniqueId,
      ExecutionInfo = new ExecutionInfo
      {
        TimeFinished = DateTime.UtcNow.ToTimestamp(),
        TimeStarted = startTime.ToTimestamp()
      }
    };

    campaignResult.ExperimentResults.AddRange(experimentResults);
  }

  private async Task<ExperimentExecutor?> GenerateExperimentExecutor(CampaignTemplate template)
  {
    // campaign template should have exactly one experiment template at this time
    var experimentTemplate = template.ExperimentTemplates.First();
    if (!experimentTemplate.IsResolved())
    {
      var resolveSuccess = await _planningHelper.TryResolveParameters(template.Planners, experimentTemplate.GetAllPlannedParameters());
      if (!resolveSuccess)
        return null;
    }
    return _experimentComposer.Compose(experimentTemplate);
  }

  public void Stop()
    => _cancellationTokenSource?.Cancel();

  public void Pause()
    => throw new NotImplementedException();

  public IObservable<bool> CanStart { get; }
}
