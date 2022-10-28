using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial;

public abstract class AresSerialPort : IAresSerialPort
{
  private readonly object _bufferLock = new();

  private readonly ISubject<(ISerialCommandWithResponse, ISerialResponse)> _responsePublisher = new Subject<(ISerialCommandWithResponse, ISerialResponse)>();
  private readonly IList<ISerialCommandWithResponse> _responseQueue = new List<ISerialCommandWithResponse>();

  protected AresSerialPort(SerialPortConnectionInfo connectionInfo)
  {
    ConnectionInfo = connectionInfo;
    DataBufferState = DataBufferStatePublisher.AsObservable();
    DataBufferState.Subscribe(ProcessBufferCore);
  }

  public IObservable<List<byte>> DataBufferState { get; }
  protected SerialPortConnectionInfo ConnectionInfo { get; }
  protected ISubject<List<byte>> DataBufferStatePublisher { get; } = new BehaviorSubject<List<byte>>(new List<byte>());

  public void AttemptOpen(string portName)
  {
    try
    {
      Open(portName);
      if (IsOpen)
        Name = portName;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  public Task<T> SendOutboundCommand<T>(SerialCommandWithResponse<T> command) where T : ISerialResponse
  {
    _responseQueue.Add(command);
    var task = _responsePublisher
      .Where(response => response.Item1 == command)
      .Select(tuple => (T)tuple.Item2)
      .Take(1)
      .ToTask();

    SendOutboundMessage(command);

    return task;
  }

  public void SendOutboundCommand(SerialCommand command)
  {
    SendOutboundMessage(command);
  }

  public abstract void Disconnect();

  public virtual void Listen()
  {
  }

  public virtual void StopListening()
  {
  }

  public string? Name { get; private set; }
  public bool IsOpen { get; protected set; }

  public abstract void SendOutboundMessage(SerialCommand command);

  private void ProcessBufferCore(List<byte> currentData)
  {
    if (!currentData.Any())
      return;

    var totalBytesRemoved = 0;
    lock ( _bufferLock )
    {
      var distinctResponseParsers = _responseQueue.DistinctBy(response => response.GetType());
      var parsedResponses = distinctResponseParsers.Select(cmd => {
          var parsed = cmd.TryParse(currentData, out var response, out var dataToRemove);
          return (Parsed: parsed, Response: response, DataToRemove: dataToRemove, CommandToRemove: cmd);
        }).Where(proxy => proxy.Parsed && proxy.Response is not null && proxy.DataToRemove is not null)
        .ToArray();


      foreach (var arrSegment in parsedResponses.Select(tuple => tuple.DataToRemove).OrderBy(bytes => bytes!.Value.Offset))
      {
        currentData.RemoveRange(arrSegment!.Value.Offset - totalBytesRemoved, arrSegment.Value.Count);
        totalBytesRemoved += arrSegment.Value.Count;
      }

      foreach (var values in parsedResponses)
      {
        _responseQueue.Remove(values.CommandToRemove);
        _responsePublisher.OnNext((values.CommandToRemove, values.Response!));
      }
    }

    if (totalBytesRemoved > 0)
      DataBufferStatePublisher.OnNext(currentData);
  }

  public void Connect(string portName)
  {
    try
    {
      AttemptOpen(portName);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }

    if (IsOpen)
      return;

    throw new InvalidOperationException(
      "Successfully attempted to open port, but the port is not reporting as open");
  }

  protected void AddDataReceived(byte[] dataReceived)
  {
    List<byte> currentData;
    lock ( _bufferLock )
    {
      currentData = DataBufferState.Take(1).Wait();
      currentData.AddRange(dataReceived);
    }

    DataBufferStatePublisher.OnNext(currentData);
  }

  // protected abstract void ProcessBuffer(ref List<byte> currentData);
  protected abstract void Open(string portName);
}
