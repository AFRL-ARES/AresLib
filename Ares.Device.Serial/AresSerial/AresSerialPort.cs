using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Ares.Device.Serial.Commands;

namespace Ares.Device.Serial;

public abstract class AresSerialPort : IAresSerialPort
{
  private readonly object _bufferLock = new();
  private readonly IList<ISerialCommandWithResponse> _multiResponseQueue = new List<ISerialCommandWithResponse>();
  private readonly ISubject<(ISerialCommandWithResponse, SerialResponse)> _responsePublisher = new Subject<(ISerialCommandWithResponse, SerialResponse)>();
  private readonly SemaphoreSlim _sendLock = new(1);
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

  public async Task<T> Send<T>(SerialCommandWithResponse<T> command, TimeSpan timeout) where T : SerialResponse
  {
    if (command is SerialCommandWithStreamedResponse<T>)
      throw new InvalidOperationException(
        "Attempted to send a command for a streamed response. Call Send instead");

    lock ( _singleResponseQueue )
    {
      _singleResponseQueue.Add(command);
    }

    var getResponseTask =
      GetTransactionStream<T>()
        .Where(transaction => transaction.Request == command)
        .Take(1)
        .Select(transaction => transaction.Response)
        .Timeout(timeout)
        .Catch(Observable.Return<T?>(null));

    await _sendLock.WaitAsync();
    SendOutboundMessage(command);

    var response = await getResponseTask;
    _sendLock.Release();
    lock ( _singleResponseQueue )
    {
      _singleResponseQueue.Remove(command);
    }

    if (response is null)
      throw new TimeoutException($"Receiving message of type {typeof(T).Name} timed out");

    return response;
  }

  public Task<T> Send<T>(SerialCommandWithResponse<T> command) where T : SerialResponse
    => Send(command, TimeSpan.FromDays(10));

  public async Task Send<T>(SerialCommandWithStreamedResponse<T> command) where T : SerialResponse
  {
    lock ( _multiResponseQueue )
    {
      var existingParser = _multiResponseQueue.OfType<SerialCommandWithStreamedResponse<T>>().FirstOrDefault();
      if (existingParser != null)
        _multiResponseQueue.Remove(existingParser);

      _multiResponseQueue.Add(command);
    }

    await _sendLock.WaitAsync();
    SendOutboundMessage(command);
    _sendLock.Release();
  }


  public IObservable<SerialTransaction<T>> GetTransactionStream<T>() where T : SerialResponse
  {
    var observable = _responsePublisher
      .Where(response => response.Item2.GetType() == typeof(T))
      .Select(tuple => new SerialTransaction<T>((SerialCommandWithResponse<T>)tuple.Item1, (T)tuple.Item2));

    return observable;
  }

  public async Task Send(SerialCommand command)
  {
    await _sendLock.WaitAsync();
    SendOutboundMessage(command);
    _sendLock.Release();
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

  private void ProcessBufferCore(List<byte> currentDataList)
  {
    if (!currentDataList.Any())
      return;

    var currentData = currentDataList.ToArray();

    var totalBytesRemoved = 0;
    lock ( _bufferLock )
    lock ( _singleResponseQueue )
    lock ( _multiResponseQueue )
    {
      var distinctResponseParsers = _singleResponseQueue.DistinctBy(response => response.ResponseParser.GetType()).ToArray();
      var unparsedMultiParsers = _multiResponseQueue.Where(multiResponseCmd => distinctResponseParsers.All(singleResponseCmd => singleResponseCmd.ResponseParser.GetType() != multiResponseCmd.ResponseParser.GetType())).ToArray();
      var considerableParsers = distinctResponseParsers.Concat(unparsedMultiParsers);
      var parsedResponses = considerableParsers.Select(cmd => {
        var parsed = cmd.ResponseParser.TryParseResponse(currentData, out var response, out var dataToRemove);
        if (parsed && response is not null)
          response.RequestId = cmd.Id;

        return (Parsed: parsed, Response: response, DataToRemove: dataToRemove, CommandToRemove: cmd);
      }).Where(proxy => proxy.Parsed && proxy.DataToRemove is not null).ToArray();

      // TODO check for overlaping arrays maybe?
      var orderedArraySegs = parsedResponses.Select(tuple => tuple.DataToRemove)
        .OrderBy(bytes => bytes!.Value.Offset)
        .Distinct();

      foreach (var arrSegment in orderedArraySegs)
      {
        currentDataList.RemoveRange(arrSegment!.Value.Offset - totalBytesRemoved, arrSegment.Value.Count);
        totalBytesRemoved += arrSegment.Value.Count;
      }

      foreach (var values in parsedResponses)
        if (values.Response is not null)
          _responsePublisher.OnNext((values.CommandToRemove, values.Response!));
    }

    if (totalBytesRemoved > 0)
      DataBufferStatePublisher.OnNext(currentDataList);
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
