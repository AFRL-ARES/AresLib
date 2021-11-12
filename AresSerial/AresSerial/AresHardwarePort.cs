using System.IO.Ports;

namespace AresSerial
{
  public class AresHardwarePort : SerialPort, IAresSerialPort
  {
    public AresHardwarePort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits) : base(portName, baudRate, parity, dataBits, stopBits)
    {
    }
  }
}
