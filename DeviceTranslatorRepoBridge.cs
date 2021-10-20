using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace AresLib
{
  public class DeviceTranslatorRepoBridge
  {
    public ISourceCache<IDeviceDomainTranslator, string> Repo { get; } = new SourceCache<IDeviceDomainTranslator, string>(translator => translator.Device.Name);
  }
}
