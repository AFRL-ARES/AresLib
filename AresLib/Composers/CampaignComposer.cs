using System.Collections.ObjectModel;
using System.Linq;
using Ares.Core;
using AresDevicePluginBase;
using AresLib.Executors;

namespace AresLib.Composers
{
  internal class CampaignComposer : CommandComposer<CampaignTemplate, CampaignExecutor>
  {
    public CampaignComposer(
      CampaignTemplate template, ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters) { }

    public override CampaignExecutor Compose()
    {
      var experimentCompilers =
        Template
          .ExperimentTemplates
          .OrderBy(template => template.Index)
          .Select(experimentTemplate => new ExperimentComposer(experimentTemplate, AvailableDeviceCommandInterpreters))
          .ToArray();

      var composedExperiments =
        experimentCompilers
          .Select(expComposer => expComposer.Compose())
          .ToArray();

      return new CampaignExecutor(composedExperiments);
    }
  }
}
