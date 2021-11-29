using System.IO.Ports;

namespace AresSerial
{
  public class SerialPortConnectionInfo
  {
    public SerialPortConnectionInfo(int baudRate,
      Parity parity,
      int dataBits,
      StopBits stopBits)
    {
      BaudRate = baudRate;
      Parity = parity;
      DataBits = dataBits;
      StopBits = stopBits;
    }

    public int BaudRate { get; set; }
    public Parity Parity { get; set; }
    public int DataBits { get; set; }
    public StopBits StopBits { get; set; }
  }
}
