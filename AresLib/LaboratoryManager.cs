using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.Device;

namespace AresLib
{
  public abstract class LaboratoryManager : ILaboratoryManager
  {
    protected abstract IDeviceCommandInterpreter<AresDevice>[] DeviceCommandInterpreters { get; }
    public Laboratory Lab { get; }
  }
}
