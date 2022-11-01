using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core.Validation.Campaign;

namespace Ares.Core.Execution.StartConditions
{
  internal class RequiredDeviceInterpretersStartCondition : IStartCondition
  {
    private readonly ICampaignValidator _requiredDeviceCommandInterpretersValidator;
    private readonly IActiveCampaignTemplateStore _activeCampaignTemplateStore;
    public RequiredDeviceInterpretersStartCondition(IActiveCampaignTemplateStore activeCampaignTemplateStore,
      IEnumerable<ICampaignValidator> validators)
    {
      _activeCampaignTemplateStore = activeCampaignTemplateStore;
      _requiredDeviceCommandInterpretersValidator = validators.OfType<RequiredDeviceInterpretersValidator>().First();
    }
    public StartConditionResult? CanStart()
    {
      var validationResult =
        _requiredDeviceCommandInterpretersValidator.Validate(_activeCampaignTemplateStore.CampaignTemplate);
      var startConditionResult = new StartConditionResult(validationResult.Success, validationResult.Messages);
      return startConditionResult;
    }
  }
}
