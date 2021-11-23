using AresDevicePluginBase;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace AresSerial
{
  public abstract class SerialDevice : AresDevice
  {
    private string TargetPortName { get; set; }
    protected SerialDevice(string name, ISerialConnection connection) : base(name)
    {
      Connection = connection;
    }

    public void Connect(string portName)
    {
      TargetPortName = portName;
      Connection.Connect(portName);
    }

    public void Disconnect()
    {
      Connection.Disconnect();
    }

    public override async Task<bool> Activate()
    {
      if (Connection == null)
      {
        throw new Exception($"Cannot activate serial device before providing connection.");
      }

      if (Connection.Port.IsOpen)
      {
        if (!Connection.ListenerCancellationTokenSource.IsCancellationRequested)
        {
          return true;
        }
      }
      ConnectionStatus connectionStatus;
      if (!Connection.Port.IsOpen)
      {
        connectionStatus = await Connection.ConnectionStatusUpdates.Take(1)
                                           .ToTask();
        Connect(TargetPortName);
        if (connectionStatus != ConnectionStatus.Connected)
        {
          throw new Exception
            (
             $"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatus}"
            );
        }
      }

      if (Connection.ListenerCancellationTokenSource?.IsCancellationRequested ?? true)
      {
        Connection.StartListening();
      }
      var listenerStatus = await Connection.ListenerStatusUpdates.Take(1).ToTask();
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

      await Task.Run(() => Connection.SendAndWaitForReceipt(validationRequest));
      var response = await responseWaiter;
      if (validationRequest is SimSerialCommandRequest simRequest)
      {
        validationRequest = simRequest.ActualRequest;
      }

      if (response.SourceRequest != validationRequest)
      {
        throw new Exception($"Received an unexpected response for {validationRequest.Serialize()}\nResponse for: {response.SourceRequest.Serialize()}\nResponse: {response.Message}");
      }

      listenerStatus = await Connection.ListenerStatusUpdates.Take(1).ToTask();

      return listenerStatus != ListenerStatus.Paused;
    }

    public ISerialConnection Connection { get; private set; }
  }
}
