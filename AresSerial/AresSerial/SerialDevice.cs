using AresDevicePluginBase;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public abstract class SerialDevice<TConnection> : AresDevice, ISerialDevice<TConnection> where TConnection : ISerialConnection
  {

    protected SerialDevice(string name) : base(name)
    {
    }

    public override async Task<bool> Activate()
    {
      if (Connection == null)
      {
        throw new Exception($"Cannot activate serial device before establishing connection. Try calling {nameof(EstablishConnection)}");
      }
      var connectionStatus = await Connection.ConnectionStatusUpdates.Take(1);
      if (connectionStatus != ConnectionStatus.Unattempted)
      {
        throw new Exception("This really shouldn't ever be thrown, probably only used for testing purposes");
      }

      Connection.Connect();
      connectionStatus = await Connection.ConnectionStatusUpdates.Take(1);
      if (connectionStatus != ConnectionStatus.Connected)
      {
        throw new Exception($"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatus}");
      }

      Connection.StartListening();
      await Task.Delay(1000);
      var listenerStatus = await Connection.ListenerStatusUpdates.Take(1);
      if (listenerStatus != ListenerStatus.Listening)
      {
        return false;
      }

      var validationRequest = Connection.GenerateValidationRequest();

      var responseWaiter =
        Connection
          .Responses
          .Take(1)
          .ToTask();

      var timeout = TimeSpan.FromSeconds(5);
      var send = Connection.SendCommand(validationRequest);
      var sendTimeout = Task.Delay(timeout);
      var completedSend = await Task.WhenAny(send, sendTimeout);
      if (completedSend == sendTimeout)
      {
        throw new TimeoutException($"Sending validation request timed out after {timeout} for device {Name}, on thread {Thread.CurrentThread.Name}");
      }

      var responseTimeout = Task.Delay(timeout);
      var fasterTask = await Task.WhenAny(responseWaiter, responseTimeout);
      if (fasterTask == responseTimeout)
      {
        throw new TimeoutException($"Did not receive an expected valid response within {timeout} for device {Name}");
      }

      var response = responseWaiter.Result;
      if (validationRequest is SimSerialCommandRequest simRequest)
      {
        validationRequest = simRequest.ActualRequest;
      }

      if (response.SourceRequest != validationRequest)
      {
        throw new Exception($"Received an unexpected response for {validationRequest.Serialize()}\nResponse for: {response.SourceRequest.Serialize()}\nResponse: {response.Message}");
      }

      listenerStatus = await Connection.ListenerStatusUpdates.Take(1);

      return listenerStatus != ListenerStatus.Paused;
    }

    public abstract TConnection EstablishConnection(string portName);

    protected TConnection Connection { get; set; }
  }
}
