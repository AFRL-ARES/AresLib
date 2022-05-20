using System.IO.Ports;

namespace Ares.Device.Serial;

public class SerialPortConnectionInfo
{
  public SerialPortConnectionInfo
    (
    int baudRate,
    Parity parity,
    int dataBits,
    StopBits stopBits,
    string entryEnding
    )
  {
    BaudRate = baudRate;
    Parity = parity;
    DataBits = dataBits;
    StopBits = stopBits;
    EntryEnding = entryEnding;
  }

  public int BaudRate { get; set; }
  public Parity Parity { get; set; }
  public int DataBits { get; set; }
  public StopBits StopBits { get; set; }
  public string EntryEnding { get; set; }
}