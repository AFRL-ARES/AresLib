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

  public bool MultilineResponse { get; set; }

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

    SystemPort.NewLine = CommandEnding;
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

    unopenedCopy.NewLine = SystemPort.NewLine;
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

    // set the timeout to a reasonable number so we can periodically time out and check to see if our
    // cancellation token has been cancelled
    if (SystemPort.ReadTimeout == SerialPort.InfiniteTimeout)
      SystemPort.ReadTimeout = 5000;

    var listenerTask = Task.Factory.StartNew(
      () => {
        while (!cancellationToken.IsCancellationRequested)
          try
          {
            var message = SystemPort.ReadLine();
            // the following would only get hit when there's a multiline response coming from the system port
            while (SystemPort.BytesToRead > 0)
            {
              var anotherLine = SystemPort.ReadLine();
              message = $"{message}{Environment.NewLine}{anotherLine}";
              // we give a bit of time to repopulate the buffer as reading too fast might
              // leave the BytesToRead at 0 while there's still data coming in
              Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).Wait(cancellationToken);
            }

            InboundMessagesPublisher.OnNext(message);
          }
          catch (TimeoutException)
          {
            cancellationToken.ThrowIfCancellationRequested();
          }
      },
      cancellationToken,
      TaskCreationOptions.LongRunning,
      TaskScheduler.Current
    );

    await listenerTask;
  }

  public void SendOutboundMessage(string input)
  {
    if (!IsOpen || SystemPort is null)
      throw new InvalidOperationException("Cannot send message as the serial port is not open.");

    SystemPort.WriteLine($"{input}");
    OutboundMessagesPublisher.OnNext(input);
  }
}
