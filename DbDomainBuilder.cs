using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class DbDomainBuilder<AppDomain,DbDomain> : IDbDomainBuilder<AppDomain,DbDomain>
  {
    public abstract DbDomain Build();

    public AppDomain Source { get; init; }
  }
}
