using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  public interface IDeviceDomainTranslator
  {
    Task GetDeviceCommand(AresCommand aresCommand);
  }
}
