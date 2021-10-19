using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class Campaign
  {
    public Guid Id { get; init; } = Guid.NewGuid();
    public Experiment[] Experiments { get; init; }
  }
}
