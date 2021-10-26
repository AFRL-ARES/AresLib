using System.Linq;
using Ares.Core;
using AresLib.Compilers;

namespace AresLib.Composers
{
  internal class CampaignComposer : CommandComposer<CampaignTemplate, CampaignExecutor>
  {
    public override CampaignExecutor Compose()
    {
      var experimentCompilers =
        Template
          .ExperimentTemplates
          .Select(experimentTemplate =>
                    new ExperimentComposer
                    {
                      Template = experimentTemplate,
                      CommandNamesToInterpreters = CommandNamesToInterpreters
                    })
          .ToArray();

      var composedExperiments =
        experimentCompilers
          .Select(expComposer => expComposer.Compose())
          .ToArray();

      return new CampaignExecutor { Experiments = composedExperiments };
    }
  }
}
