using System;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public class AresHardwarePort : IAresSerialPort
  {
    private ISubject<string> OutboundMessagesPublisher { get; } = new Subject<string>();
    private ISubject<string> InboundMessagesPublisher { get; } = new Subject<string>();
    private SerialPort SystemPort { get; }

    public AresHardwarePort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
    {
      SystemPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
      OutboundMessages = OutboundMessagesPublisher.AsObservable();
      InboundMessages = InboundMessagesPublisher.AsObservable();
    }

    public string Name
    {
      get => SystemPort.PortName;
    }

    public bool IsOpen
    {
      get => SystemPort.IsOpen;
    }

    public IObservable<string> OutboundMessages { get; }

    public IObservable<string> InboundMessages { get; }

    public void Open() => SystemPort.Open();

    public async Task ListenForEntryAsync(CancellationToken cancellationToken)
    {
      var readTimeout = SystemPort.ReadTimeout;
      if (readTimeout == SerialPort.InfiniteTimeout)
      {
        readTimeout = 1000;
      }

      await Task.Run
        (
         async () =>
         {
           Thread.CurrentThread.Name ??= $"{Name} Inbound Message";
           while (!cancellationToken.IsCancellationRequested)
           {
             try
             {
               var inboundMessage = SystemPort.ReadLine();
               InboundMessagesPublisher.OnNext(inboundMessage);
             }
             catch (Exception e)
             {
               cancellationToken.ThrowIfCancellationRequested();
               await Task.Delay(readTimeout);
             }
           }
         }
        );
    }

    public void SendOutboundMessage(string input)
    {
      SystemPort.WriteLine(input);
      OutboundMessagesPublisher.OnNext(input);
    }
  }
}
