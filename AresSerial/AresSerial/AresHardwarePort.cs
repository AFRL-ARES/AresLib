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
    private ISubject<string> OutboundMessagesPublisher { get; set; } = new Subject<string>();
    private ISubject<string> InboundMessagesPublisher { get; set; } = new Subject<string>();
    private SerialPort SystemPort { get; set; }

    public AresHardwarePort(int baudRate, Parity parity, int dataBits, StopBits stopBits)
    {
      SystemPort = new SerialPort(null, baudRate, parity, dataBits, stopBits);
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

    public IObservable<string> OutboundMessages { get; private set; }

    public IObservable<string> InboundMessages { get; private set; }

    public void Open(string portName)
    {
      // Make sure the port isn't already connected?
      SystemPort.PortName = portName;
      SystemPort.Open();
    }

    public void Close(Exception error = null)
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

      var unopenedCopy = new SerialPort(null, SystemPort.BaudRate, SystemPort.Parity, SystemPort.DataBits, SystemPort.StopBits);
      SystemPort.Close();
      OutboundMessagesPublisher = new Subject<string>();
      InboundMessagesPublisher = new Subject<string>();
      SystemPort = unopenedCopy;
      OutboundMessages = OutboundMessagesPublisher.AsObservable();
      InboundMessages = InboundMessagesPublisher.AsObservable();
    }

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
