using System;
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

    OutboundMessages = OutboundMessagesPublisher.AsObservable();
    InboundMessages = InboundMessagesPublisher.AsObservable();
  }

  private ISubject<string> OutboundMessagesPublisher { get; set; } = new Subject<string>();
  private ISubject<string> InboundMessagesPublisher { get; set; } = new Subject<string>();
  private SerialPort? SystemPort { get; set; }

  public string CommandEnding => _connectionInfo.EntryEnding;

  public string Name => SystemPort?.PortName ?? string.Empty;

  public bool IsOpen => SystemPort?.IsOpen ?? false;

  public IObservable<string> OutboundMessages { get; private set; }

  public IObservable<string> InboundMessages { get; private set; }

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

  public void Close(Exception? error = null)
  {
    if (error == null)
    {
      OutboundMessagesPublisher.OnCompleted();
      InboundMessagesPublisher.OnCompleted();
    }
    else
    {
      OutboundMessagesPublisher.OnError(error);
      InboundMessagesPublisher.OnError(error);
    }

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
    OutboundMessagesPublisher = new Subject<string>();
    InboundMessagesPublisher = new Subject<string>();
    SystemPort = unopenedCopy;
    OutboundMessages = OutboundMessagesPublisher.AsObservable();
    InboundMessages = InboundMessagesPublisher.AsObservable();
  }

  public async Task ListenForEntryAsync(CancellationToken cancellationToken)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot listen as the serial port is not open.");

    var readTimeout = SystemPort.ReadTimeout;
    if (readTimeout == SerialPort.InfiniteTimeout)
      readTimeout = 1000;

    await Task.Run(
      async () => {
        while (!cancellationToken.IsCancellationRequested)
          try
          {
            var inboundMessage = SystemPort.ReadLine();
            InboundMessagesPublisher.OnNext(inboundMessage);
          }
          catch (TimeoutException)
          {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(readTimeout, cancellationToken);
          }
      },
      cancellationToken
    );
  }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    SystemPort.WriteLine($"{input}{CommandEnding}");
    OutboundMessagesPublisher.OnNext(input);
  }
}
