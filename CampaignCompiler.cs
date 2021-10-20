using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class CampaignCompiler : ExecutableCompiler, ICampaignCompiler
  {

    public override Task GenerateExecutable()
    {
      var experimentCompilers =
        Campaign
          .Experiments
          .Select(experiment => new ExperimentCompiler())
          .ToArray();

      var experimentExecutables =
        experimentCompilers
          .Select(expCompiler => expCompiler.GenerateExecutable())
          .ToArray();

      return experimentExecutables
        .Aggregate(
                   async (current, next) =>
                   {
                     await current;
                     await next;
                   });
    }

    public Campaign Campaign { get; init; }
  }
}
