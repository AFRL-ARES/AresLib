using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal interface ICampaignCompiler : IExecutableCompiler
  {
    Campaign Campaign { get; init; }
  }
}
