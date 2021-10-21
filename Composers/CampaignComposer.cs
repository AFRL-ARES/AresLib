using Ares.Core;
using System.Linq;
using System.Threading.Tasks;

namespace AresLib
{
  internal class CampaignComposer : CommandComposer<CampaignTemplate>
  {
    public override Task Compose()
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

      return composedExperiments
        .Aggregate(
                   async (current, next) =>
                   {
                     await current;
                     await next;
                   });
    }
  }
}
