using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  public abstract class AresDevice : IAresDevice
  {

    protected abstract IDeviceDomainTranslator DomainTranslator { get; }
  }
}
