using System;
using System.Linq;

namespace AresLib
{
  internal class CampaignBuilder : IBuilder<Campaign>
  {
    public int NumExperiments { get; set; }
    public ExperimentBuilder ExperimentBuilder { get; set; }
    public Campaign Build()
    {
      return new Campaign
      {
        Experiments = Enumerable.Range(1, NumExperiments).Select(_ => ExperimentBuilder.Build()).ToArray()
      };
    }
  }
}
