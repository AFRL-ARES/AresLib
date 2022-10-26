using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ares.Device.Serial
{
  internal interface ISerialDeviceInternal : ISerialDevice
  {
    void Connect(IAresSerialPort serialPort);
  }
}
