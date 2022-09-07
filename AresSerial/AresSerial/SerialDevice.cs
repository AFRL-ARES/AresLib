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
      return;

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
  }

  public override async Task<bool> Activate()
  {
    if (Connection is null)
      throw new Exception("Cannot activate serial device before providing connection.");

    if (await Connection.ConnectionStatusUpdates.FirstAsync() == ConnectionStatus.Connected
        && await Connection.ListenerStatusUpdates.FirstAsync() == ListenerStatus.Listening)
      return true;

    if (await Connection.ConnectionStatusUpdates.FirstAsync() != ConnectionStatus.Connected)
    {
      var connectionStatusListener = Connection.ConnectionStatusUpdates.FirstAsync().ToTask();
      Connect();
      var connectionStatus = connectionStatusListener.Result;

      if (connectionStatus != ConnectionStatus.Connected)
      {
        StatusPublisher.OnNext(new DeviceStatus
        {
          DeviceState = DeviceState.Error,
          Message = $"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatus}"
        });

        throw new Exception
        (
          $"Failing to connect should result in {ConnectionStatus.Failed} and throw before this statement, or be {ConnectionStatus.Connected}. Result is {connectionStatus}"
        );
      }
    }

    if (await Connection.ListenerStatusUpdates.FirstAsync() != ListenerStatus.Listening)
      Connection.StartListening();

    var listeningRetries = 5;
    var listenerStatus = await Connection.ListenerStatusUpdates.Take(1).ToTask();
    while (listenerStatus != ListenerStatus.Listening)
    {
      await Task.Delay(TimeSpan.FromMilliseconds(200));
      listenerStatus = await Connection.ListenerStatusUpdates.Take(1).ToTask();
      if (listeningRetries-- == 0)
        break;
    }

    if (listenerStatus != ListenerStatus.Listening)
      return false;

    if (!await Validate())
      return false;

    listenerStatus = await Connection.ListenerStatusUpdates.Take(1).ToTask();

    return listenerStatus != ListenerStatus.Paused;
  }

  protected abstract Task<bool> Validate();
}
