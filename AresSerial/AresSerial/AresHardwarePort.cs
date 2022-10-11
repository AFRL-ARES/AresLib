using System;
using System.IO;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal class AresHardwarePort : IAresSerialPort
{
  private readonly SerialPortConnectionInfo _connectionInfo;

  public AresHardwarePort(SerialPortConnectionInfo connectionInfo)
  {
    _connectionInfo = connectionInfo;

    DataReceived = DataPublisher.AsObservable();
  }


  public void Open(string portName)
  {
    // Make sure the port isn't already connected?
    if (SystemPort is not null && SystemPort.IsOpen)
      return;

    SystemPort = new SerialPort(
      portName,
      _connectionInfo.BaudRate,
      _connectionInfo.Parity,
      _connectionInfo.DataBits,
      _connectionInfo.StopBits
    );

    SystemPort.Open();
  }

  public void Listen()
  {
    SystemPort.DataReceived += ProcessReceivedData;
  }

  public void StopListening()
  {
    SystemPort.DataReceived -= ProcessReceivedData;
  }

  private void ProcessReceivedData(object sender, SerialDataReceivedEventArgs e)
  {
    var port = (SerialPort)sender;
    var receivedData = port.ReadExisting();
    DataPublisher.OnNext(receivedData);
  }


  public void Close(Exception? error = null)
  {
    if (SystemPort is null)
      return;

    var unopenedCopy = new SerialPort(
      SystemPort.PortName,
      SystemPort.BaudRate,
      SystemPort.Parity,
      SystemPort.DataBits,
      SystemPort.StopBits
    );

    SystemPort.Close();
    SystemPort = unopenedCopy;
  }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    SystemPort.Write(input);
  }

  private ISubject<string> DataPublisher { get; set; } = new Subject<string>();
  private SerialPort? SystemPort { get; set; }

  public string Name => SystemPort?.PortName ?? string.Empty;

  public bool IsOpen => SystemPort?.IsOpen ?? false;

  public IObservable<string> DataReceived { get; }
}
