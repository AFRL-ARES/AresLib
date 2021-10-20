using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class CommandDomainCoupling
  {
    public DeviceCommand AppDomain { get; init; }
    public AresCommand DbDomain { get; init; }
  }
}
