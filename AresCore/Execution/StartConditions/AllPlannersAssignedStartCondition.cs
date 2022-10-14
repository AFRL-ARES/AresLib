using Ares.Core.Execution.Extensions;

namespace Ares.Core.Execution.StartConditions;

internal class AllPlannersAssignedStartCondition : IStartCondition
{
  private readonly IActiveCampaignTemplateStore _activeCampaignTemplateStore;

  public AllPlannersAssignedStartCondition(IActiveCampaignTemplateStore activeCampaignTemplateStore)
  {
    _activeCampaignTemplateStore = activeCampaignTemplateStore;
  }

  public string Message
  {
    get
    {
      if (_activeCampaignTemplateStore.CampaignTemplate is null)
        return "Campaign Template has not been assigned for proper message";

      var plannableParameters = _activeCampaignTemplateStore.CampaignTemplate.ExperimentTemplates.First().GetAllPlannedParameters();
      var paramsWithoutPlanner = plannableParameters.Where(parameter => _activeCampaignTemplateStore.CampaignTemplate.PlannerAllocations.All(allocation => allocation.Parameter.UniqueId != parameter.PlanningMetadata.UniqueId));
      var paramsWIthoutPlannerNames = paramsWithoutPlanner.Select(parameter => parameter.PlanningMetadata.Name);
      return $"Parameters [{string.Join(", ", paramsWIthoutPlannerNames)}] do not have planners assigned.";
    }
  }

  public bool CanStart()
  {
    // technically null campaign template would indicate that the campaign should not be run
    // but in the context of this start condition, returning false would indicate that not
    // all plannable parameters have values assigned which is also untrue as there is not template
    // TODO maybe make the return value nullable for such cases?
    if (_activeCampaignTemplateStore.CampaignTemplate is null)
      return true;

    var plannableParameters = _activeCampaignTemplateStore.CampaignTemplate.ExperimentTemplates.First().GetAllPlannedParameters();
    return plannableParameters.All(parameter => _activeCampaignTemplateStore.CampaignTemplate.PlannerAllocations.Any(allocation => allocation.Parameter.UniqueId == parameter.PlanningMetadata.UniqueId));
  }
}
