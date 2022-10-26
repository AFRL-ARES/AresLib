using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

public class AresHardwarePort : AresSerialPort
{
  protected AresHardwarePort(SerialPortConnectionInfo connectionInfo) : base(connectionInfo)
  {
  }

  protected override void ProcessBuffer(ref List<byte> currentData)
  {
    throw new NotImplementedException();
  }

  protected override void Open(string portName)
  {
    // Make sure the port isn't already connected?
    if (SystemPort is not null && SystemPort.IsOpen)
      return;

    SystemPort = new SerialPort(
      portName,
      ConnectionInfo.BaudRate,
      ConnectionInfo.Parity,
      ConnectionInfo.DataBits,
      ConnectionInfo.StopBits
    );

    SystemPort.Open();
    IsOpen = SystemPort.IsOpen;
  }

  public override void Disconnect()
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

  private void ProcessReceivedData(object sender, SerialDataReceivedEventArgs e)
  {
    var port = (SerialPort)sender;
    var buffer = new byte[port.BytesToRead];
    port.Read(buffer, 0, buffer.Length);
    AddDataReceived(buffer);
  }

  public override void Listen()
  {
    SystemPort.DataReceived += ProcessReceivedData;
  }

  public override void StopListening()
  {
    SystemPort.DataReceived -= ProcessReceivedData;
  }

  public override void SendOutboundMessage(byte[] input)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    SystemPort.Write(input, 0, input.Length);
  }

  private SerialPort? SystemPort { get; set; }
}