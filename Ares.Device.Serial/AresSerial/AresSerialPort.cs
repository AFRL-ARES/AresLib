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
  private readonly IList<ISerialCommandWithResponse> _multiResponseQueue = new List<ISerialCommandWithResponse>();
  private readonly ISubject<(ISerialCommandWithResponse, SerialResponse)> _responsePublisher = new Subject<(ISerialCommandWithResponse, SerialResponse)>();
  private readonly IList<ISerialCommandWithResponse> _singleResponseQueue = new List<ISerialCommandWithResponse>();

  protected internal AresSerialPort(SerialPortConnectionInfo connectionInfo)
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
      if (!IsOpen)
        throw new InvalidOperationException($"Successfully executed Open on {portName}, but did not report IsOpen");

      Name = portName;
      Listen();
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  public Task<T> Send<T>(SerialCommandWithResponse<T> command) where T : SerialResponse
  {
    var streamedResponseCommand = command as SerialCommandWithStreamedResponse<T>;
    if (streamedResponseCommand != null)
      throw new InvalidOperationException(
        "Attempted to send a command for a streamed response. Call AttemptSendDistinct instead");

    _singleResponseQueue.Add(command);
    var response = _responsePublisher
      .Where(tuple => tuple.Item1 == command)
      .Take(1)
      .Select(transaction => (T)transaction.Item2).ToTask();

    SendOutboundMessage(command);
    return response;
  }

  public void AttemptSendDistinct<T>(SerialCommandWithStreamedResponse<T> command) where T : SerialResponse
  {
    var existsInMultiQueue = _multiResponseQueue.OfType<SerialCommandWithStreamedResponse<T>>().Any();
    if (existsInMultiQueue)
      throw new InvalidOperationException($"Command of type {command.GetType().Name} already persisting.");

    _multiResponseQueue.Add(command);
    // if (_singleResponseQueue.All(singleCommandWithResponse => singleCommandWithResponse.GetType() != command.GetType()))
    SendOutboundMessage(command);
  }


  public IObservable<SerialTransaction<T>> GetTransactionStream<T>() where T : SerialResponse
  {
    var observable = _responsePublisher
      .Where(response => response.Item2.GetType() == typeof(T))
      .Select(tuple => new SerialTransaction<T>((SerialCommandWithResponse<T>)tuple.Item1, (T)tuple.Item2));

    return observable;
  }

  public void Send(SerialCommand command)
  {
    SendOutboundMessage(command);
  }


  public void Close()
  {
    StopListening();
    CloseCore();
    if (!IsOpen)
      return;

    throw new InvalidOperationException("Successfully executed Close, but did not report IsOpen as false");
  }

  public string? Name { get; private set; }
  public bool IsOpen { get; protected set; }

  protected internal abstract void CloseCore();

  public virtual void Listen()
  {
  }

  public virtual void StopListening()
  {
  }

  public abstract void SendOutboundMessage(SerialCommand command);

  private void ProcessBufferCore(List<byte> currentData)
  {
    if (!currentData.Any())
      return;

    var totalBytesRemoved = 0;
    lock ( _bufferLock )
    {
      var distinctResponseParsers = _singleResponseQueue.DistinctBy(response => response.GetType()).ToArray();
      var unparsedMultiParsers = _multiResponseQueue.Where(multiResponseCmd => distinctResponseParsers.All(singleResponseCmd => singleResponseCmd.ResponseParser.GetType() != multiResponseCmd.ResponseParser.GetType())).ToArray();
      var considerableParsers = distinctResponseParsers.Concat(unparsedMultiParsers);
      var parsedResponses = considerableParsers.Select(cmd => {
        var parsed = cmd.ResponseParser.TryParseResponse(currentData, out var response, out var dataToRemove);
        if (parsed && response is not null)
          response.RequestId = cmd.Id;

        return (Parsed: parsed, Response: response, DataToRemove: dataToRemove, CommandToRemove: cmd);
      }).Where(proxy => proxy.Parsed && proxy.Response is not null && proxy.DataToRemove is not null).ToArray();

      var orderedArraySegs = parsedResponses.Select(tuple => tuple.DataToRemove).OrderBy(bytes => bytes!.Value.Offset).ToArray();
      foreach (var arrSegment in orderedArraySegs)
      {
        currentData.RemoveRange(arrSegment!.Value.Offset - totalBytesRemoved, arrSegment.Value.Count);
        totalBytesRemoved += arrSegment.Value.Count;
      }

      foreach (var values in parsedResponses)
      {
        _singleResponseQueue.Remove(values.CommandToRemove);
        _responsePublisher.OnNext((values.CommandToRemove, values.Response!));
      }
    }

    if (totalBytesRemoved > 0)
      DataBufferStatePublisher.OnNext(currentData);
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

  protected abstract void Open(string portName);
}
