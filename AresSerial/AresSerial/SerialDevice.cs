using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Ares.Messaging.Device;

namespace Ares.Device.Serial;

public abstract class SerialDevice : AresDevice
{
  protected SerialDevice(string name, ISerialConnection connection) : base(name)
  {
    Connection = connection;
  }

  private string? TargetPortName { get; set; }

  public ISerialConnection Connection { get; }

  public void Connect(string portName)
  {
    TargetPortName = portName;
    Connect();
  }

  private void Connect()
  {
    if (TargetPortName is null)
    {

      return;
    }

    try
    {
      Connection.Connect(TargetPortName);
    }
    catch (Exception e)
    {
      StatusPublisher.OnNext(new DeviceStatus { DeviceState = DeviceState.Error, Message = e.Message });
      throw;
    }

    StatusPublisher.OnNext(new DeviceStatus { DeviceState = DeviceState.Active });
  }

  public void Disconnect()
  {
    Connection.Disconnect();
    var connectionStatusUpdate = Connection.ConnectionStatusUpdates.Take(1).ToTask();
    connectionStatusUpdate.Wait();
    var connectionStatus = connectionStatusUpdate.Result;
    if (connectionStatus != ConnectionStatus.Connected)
    {
      StatusPublisher.OnNext(new DeviceStatus { DeviceState = DeviceState.Inactive });
    }
  }

  public override bool Activate()
  {
    if (Connection is null)
      throw new Exception("Cannot activate serial device before providing connection.");
    var connectionStatusUpdate = Connection.ConnectionStatusUpdates.FirstAsync().ToTask();
    var listenerStatusUpdate = Connection.ListenerStatusUpdates.FirstAsync().ToTask();
    connectionStatusUpdate.Wait();
    listenerStatusUpdate.Wait();
    if (connectionStatusUpdate.Result  == ConnectionStatus.Connected
        && listenerStatusUpdate.Result  == ListenerStatus.Listening)
      return true;

    connectionStatusUpdate = Connection.ConnectionStatusUpdates.FirstAsync().ToTask();
    connectionStatusUpdate.Wait();
    if (connectionStatusUpdate.Result != ConnectionStatus.Connected)
    {
      connectionStatusUpdate = Connection.ConnectionStatusUpdates.FirstAsync().ToTask();
      Connect();
      connectionStatusUpdate.Wait();
      if (connectionStatusUpdate.Result != ConnectionStatus.Connected)
      {
        StatusPublisher.OnNext(new DeviceStatus
        {
          DeviceState = DeviceState.Error,
          Message = $"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatusUpdate.Result}"
        });

        throw new Exception
        (
          $"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatusUpdate.Result}"
        );
      }
    }

    listenerStatusUpdate = Connection.ListenerStatusUpdates.FirstAsync().ToTask();
    listenerStatusUpdate.Wait();
    if (listenerStatusUpdate.Result != ListenerStatus.Listening)
      Connection.StartListening();

    var listeningRetries = 5;
    listenerStatusUpdate = Connection.ListenerStatusUpdates.Take(1).ToTask();
    listenerStatusUpdate.Wait();
    while (listenerStatusUpdate.Result != ListenerStatus.Listening)
    {
      Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
      listenerStatusUpdate = Connection.ListenerStatusUpdates.Take(1).ToTask();
      listenerStatusUpdate.Wait();
      if (listeningRetries-- == 0)
        break;
    }

    if (listenerStatusUpdate.Result != ListenerStatus.Listening)
      return false;

    if (!Validate())
      return false;

    listenerStatusUpdate = Connection.ListenerStatusUpdates.Take(1).ToTask();
    listenerStatusUpdate.Wait();

    return listenerStatusUpdate.Result != ListenerStatus.Error;
  }

  protected abstract bool Validate();
}
