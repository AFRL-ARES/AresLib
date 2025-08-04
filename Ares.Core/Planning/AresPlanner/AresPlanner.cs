using Ares.Messaging;
using Ares.Messaging.Analyzing;
using Ares.Tools;
using AresPlanner;
using Google.Protobuf.WellKnownTypes;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ares.Core.Planning.AresPlanner;

public class AresPlanner : IPlanner
{
  private readonly ISubject<PlannerState> _plannerStateSubject = new BehaviorSubject<PlannerState>(Planning.PlannerState.Disconnected);
  readonly Uri _address;

  public AresPlanner(string name, Uri address)
  {
    _address = address;
    Name = name;
    Address = address.OriginalString;
    PlannerState = _plannerStateSubject.AsObservable();
    UniqueId = Guid.NewGuid().ToString();
  }

  public async Task<IEnumerable<PlanResult>> Plan(IEnumerable<ParameterMetadata> plannableParameters, IEnumerable<CompletedExperiment> completedExperiments, IEnumerable<Analysis> _experimentAnalyses, CancellationToken cancellationToken)
  {
    var client = ClientStore.AresPlanningClient;
    var planRequest = new PlanRequest();
    planRequest.PlanningParameters.AddRange(plannableParameters.Select(parameter => ConvertToPlanningParameter(parameter, completedExperiments)));
    var result = await client.PlanAsync(planRequest, deadline: DateTime.UtcNow.AddSeconds(30));
    return ToPlanResults(result, plannableParameters);
  }

  public IEnumerable<PlanResult> ToPlanResults(PlanResponse result, IEnumerable<ParameterMetadata> plannableMetadata)
  {
    var planResults = new List<PlanResult>();

    if(result.ParameterValues.Count() != result.ParameterNames.Count())
      return planResults;

    for(int i = 0; i < result.ParameterNames.Count; i++)
    {
      var matchingMetadata = plannableMetadata.FirstOrDefault(data => data.Name == result.ParameterNames[i]);

      //What do we do if we don't find the old metadata?
      if(matchingMetadata is null)
      {
        matchingMetadata = new ParameterMetadata();
        matchingMetadata.Name = result.ParameterNames[i];
      }

      var valueResult = AresValueHelper.CreateNumber(result.ParameterValues[i]);
      var aresPlanResult = new PlanResult(matchingMetadata, valueResult);
      planResults.Add(aresPlanResult);
    }

    return planResults;
  }

  public PlanningParameter ConvertToPlanningParameter(ParameterMetadata metadata, IEnumerable<CompletedExperiment> experimentHistory)
  {
    var relevantInfo = experimentHistory.SelectMany(experiment => experiment.Parameters.Where(param => param.PlanningMetadata.Name == metadata.Name));
    var parameter = new PlanningParameter
    {
      ParameterName = metadata.Name,
      IsPlanned = true,
      DataType = metadata.GetType().ToString()
    };
    parameter.ParameterHistory.AddRange(relevantInfo.Select(param => double.Parse(param.Value.Value.StringValue)));

    if(metadata.Constraints.Any())
    {
      var constraint = metadata.Constraints.First();
      parameter.MinimumValue = constraint.Minimum;
      parameter.MaximumValue = constraint.Maximum;
    }

    return parameter;
  }

  public void Init()
  {
    ClientStore.CreateClient(_address);
    _plannerStateSubject.OnNext(Planning.PlannerState.Connected);
  }

  public string Name { get; set; }
  public Version Version { get; set; } = new Version(1, 0);
  public IObservable<PlannerState> PlannerState { get; }
  public string Address { get; set; }
  public string UniqueId { get; set; }
}
