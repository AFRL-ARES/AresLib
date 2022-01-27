using System.Collections.ObjectModel;
using Ares.Core.Executors;
using Ares.Device;
using Ares.Messaging;

namespace Ares.Core.Composers;

internal class CampaignComposer : CommandComposer<CampaignTemplate, CampaignExecutor>
{
  public CampaignComposer(
    CampaignTemplate template,
    ReadOnlyObservableCollection<IDeviceCommandInterpreter<IAresDevice>> availableDeviceCommandInterpreters) : base(template, availableDeviceCommandInterpreters)
  {
  }

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
