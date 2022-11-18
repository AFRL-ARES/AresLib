using System;
using System.IO.Ports;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial;

public class AresHardwarePort : AresSerialPort
{
  protected AresHardwarePort(SerialPortConnectionInfo connectionInfo, string portName, TimeSpan? sendBuffer = null) : base(connectionInfo, portName, sendBuffer)
  {
  }

  private SerialPort? SystemPort { get; set; }

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

  protected internal override void CloseCore()
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
    IsOpen = SystemPort.IsOpen;
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

  protected override void SendOutboundMessage(SerialCommand command)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    var serializedData = command.SerializedData;
    SystemPort.Write(serializedData, 0, serializedData.Length);
  }
}
