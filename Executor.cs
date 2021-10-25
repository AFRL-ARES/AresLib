using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Composers;

namespace AresLib
{
  public class Executor
  {
    public Task ExecuteTemplate(CampaignTemplate template)
    {
      var composer = new CampaignComposer
      {
        Template = template
      };

      var peepee = composer.Compose();
      return peepee.Execute();
    }
  }
}
