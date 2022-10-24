using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Ares.Device.Serial.Simulation;

namespace Ares.Device.Serial;

public abstract class SerialConnection : ISerialConnection
{
  private IDisposable _listener = Disposable.Empty;
  private ISubject<string> DataBufferStatePublisher { get; }
  private ISubject<ConnectionStatus> ConnectionStatusUpdatesSource { get; } = new BehaviorSubject<ConnectionStatus>(ConnectionStatus.Unattempted);
  private ISubject<ListenerStatus> ListenerStatusUpdatesSource { get; } = new BehaviorSubject<ListenerStatus>(ListenerStatus.Idle);
  private ISubject<string> ResponsePublisher { get; set; } = new Subject<string>();
  private IAresSerialPort Port { get; }

  private SerialConnection(IAresSerialPort port)
  {
    Port = port;
    ConnectionStatusUpdates = ConnectionStatusUpdatesSource.AsObservable();
    ListenerStatusUpdates = ListenerStatusUpdatesSource.AsObservable();
    DataBufferStatePublisher = new BehaviorSubject<string>(string.Empty);
    DataBufferState = DataBufferStatePublisher.AsObservable();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Unattempted);
    DataBufferState.Subscribe(ProcessBuffer);
  }

  protected SerialConnection(SerialPortConnectionInfo connectionInfo) : this(new AresHardwarePort(connectionInfo))
  {
  }

  protected SerialConnection(SimAresDevice simDevice) : this(new AresSimPort(simDevice.OutputChannel, simDevice.InputChannel))
  {
  }

  protected abstract string[] CollectMessageEntries(string buffer);

  protected virtual void ProcessBuffer(string currentBuffer)
  {
    var consumableMessageEntries = CollectMessageEntries(currentBuffer);
    var processedBuffer = new string(currentBuffer);
    foreach (var consumableMessageEntry in consumableMessageEntries)
    {
      processedBuffer = processedBuffer.Replace(consumableMessageEntry, string.Empty);
    }

    if (currentBuffer.Equals(processedBuffer))
    {
      return;
    }

    DataBufferStatePublisher.OnNext(processedBuffer);
  }

  private void Listen()
  {
    Port.Listen();
    _listener = Port.DataReceived.Subscribe(AddDataReceived); 
    ListenerStatusUpdatesSource.OnNext(ListenerStatus.Listening);
  }

  private void AddDataReceived(string dataReceived)
  {
    var currentBuffer = DataBufferState.Take(1).Wait();
    var updatedBuffer = currentBuffer + dataReceived;
    DataBufferStatePublisher.OnNext(updatedBuffer);
  }

  public void Connect(string portName)
  {
    try
    {
      Port.Open(portName);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Failed);
      throw;
    }

    if (!Port.IsOpen)
    {
      ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Failed);
      return;
    }

    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.Connected);
  }

  public void Disconnect()
  {
    StopListening();
    Port.Close();
    ConnectionStatusUpdatesSource.OnNext(ConnectionStatus.ManuallyClosed);
  }

  public void StartListening()
  {
    if (!Equals(_listener, Disposable.Empty))
    {
      throw new InvalidOperationException($"{Port.Name} Listener loop already started");
    }

    ResponsePublisher = new Subject<string>();
    Listen();
  }

  public void StopListening()
  {
    Port.StopListening();
    _listener.Dispose();
    _listener = Disposable.Empty;
    ListenerStatusUpdatesSource.OnNext(ListenerStatus.Idle);
    ResponsePublisher.OnCompleted();
  }

  public virtual void Send(string request) => Port.SendOutboundMessage(request);
  public virtual void Send(byte[] request) => Port.SendOutboundMessage(request);
  public IObservable<string> DataBufferState { get; }

  public IObservable<ConnectionStatus> ConnectionStatusUpdates { get; }
  public IObservable<ListenerStatus> ListenerStatusUpdates { get; }
}
