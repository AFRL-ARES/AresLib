using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ares.Device.Serial.Simulation;

public abstract class AresSimPort : AresSerialPort
{
  protected AresSimPort(SerialPortConnectionInfo connectionInfo) : base(connectionInfo)
  {
  }

  protected override void Open(string portName)
  {
    if (!portName.StartsWith("SIM"))
    {
      throw new InvalidOperationException(
        $"Tried opening simulated port of type {GetType().Name} with port name {portName}. Simulated ports may only open on ports starting with SIM");
    }

    IsOpen = true;
  }
}
