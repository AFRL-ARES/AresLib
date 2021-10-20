using System.Linq;

namespace AresLib
{
  internal class CampaignDbDomainBuilder : IDbDomainBuilder<Campaign>
  {
    public int NumExperiments { get; set; }
    public ExperimentDbDomainBuilder ExperimentDbDomainBuilder { get; set; }
    public Campaign Build()
    {
      return new Campaign
      {
        Experiments = Enumerable.Range(1, NumExperiments).Select(_ => ExperimentDbDomainBuilder.Build()).ToArray()
      };
    }
  }
}
