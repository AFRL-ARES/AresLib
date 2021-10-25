using System.Linq;
using Ares.Core;
using AresLib.Compilers;

namespace AresLib.Composers
{
  internal class CampaignComposer : CommandComposer<CampaignTemplate>
  {
    public CampaignComposer()
    {
      DeviceCommandCompilerRepoBridge = new DeviceCommandCompilerRepoBridge();
    }
    public override ExecutableCampaign Compose()
    {
      var experimentCompilers =
        Template
          .ExperimentTemplates
          .Select(experimentTemplate =>
                    new ExperimentComposer
                    {
                      Template = experimentTemplate,
                      DeviceCommandCompilerRepoBridge = DeviceCommandCompilerRepoBridge
                    })
          .ToArray();

      var composedExperiments =
        experimentCompilers
          .Select(expComposer => expComposer.Compose())
          .ToArray();

      return new ExecutableCampaign { Experiments = composedExperiments };
    }
  }
}
