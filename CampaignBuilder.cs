using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace AresLib
{
  internal class CampaignBuilder : IBuilder<Campaign>
  {
    public Campaign Build(ExperimentBuilder experimentBuilder, int numExperiments)
    {
      var campaign =
        new Campaign
        {
          Experiments = Enumerable.Range(1,numExperiments).Select(expNum => experimentBuilder.Build()).ToArray(),
        };

      return campaign;
    }
  }
}
