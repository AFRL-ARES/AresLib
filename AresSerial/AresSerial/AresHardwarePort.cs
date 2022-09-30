using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.Device.Serial;

internal class AresHardwarePort : IAresSerialPort
{
  private readonly SerialPortConnectionInfo _connectionInfo;

  public AresHardwarePort(SerialPortConnectionInfo connectionInfo)
  {
    _connectionInfo = connectionInfo;
  }

  private SerialPort? SystemPort { get; set; }

  public string CommandEnding => _connectionInfo.EntryEnding;

  public string Name => SystemPort?.PortName ?? string.Empty;

  public bool IsOpen => SystemPort?.IsOpen ?? false;

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

    if (_connectionInfo.ReadBufferSize.HasValue)
      SystemPort.ReadBufferSize = _connectionInfo.ReadBufferSize.Value;

    SystemPort.NewLine = CommandEnding;
    SystemPort.Open();
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

    unopenedCopy.NewLine = SystemPort.NewLine;
    unopenedCopy.ReadBufferSize = SystemPort.ReadBufferSize;
    SystemPort.Close();
    SystemPort = unopenedCopy;
  }

  public Task<string> ListenForEntryAsync(CancellationToken cancellationToken, TimeSpan timeout)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot listen as the serial port is not open.");

    // set the timeout to a reasonable number so we can periodically time out and check to see if our
    // cancellation token has been cancelled
    if (timeout == default)
      SystemPort.ReadTimeout = 10000;
    else
      SystemPort.ReadTimeout = (int)timeout.TotalMilliseconds;

    SystemPort.DiscardInBuffer();

    return Task.Run(
      () => {
        var test = $"{(char)SystemPort.ReadChar()}";
        test += SystemPort.ReadExisting();
        var tries = 5;
        while (tries > 0 && !cancellationToken.IsCancellationRequested)
        {
          if (SystemPort.BytesToRead == 0)
          {
            tries--;
            // we give a bit of time to repopulate the buffer as reading too fast might
            // leave the BytesToRead at 0 while there's still data coming in
            Thread.Sleep(25);
            continue;
          }

          tries = 5;
          test += SystemPort.ReadExisting();
        }

        return test;
      },
      cancellationToken
    );
  }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    SystemPort.WriteLine($"{input}");
  }
}
