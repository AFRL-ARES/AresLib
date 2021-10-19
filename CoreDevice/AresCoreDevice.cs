using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class AresCoreDevice : IAresDevice
  {

    public IDeviceDomainTranslator DomainTranslator { get; }
  }
}
