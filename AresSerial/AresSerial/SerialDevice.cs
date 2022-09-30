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
    var connectionStatusUpdate = Connection.ConnectionStatusUpdates.Take(1).ToTask();
    connectionStatusUpdate.Wait();
    var connectionStatus = connectionStatusUpdate.Result;
    if (connectionStatus != ConnectionStatus.Connected)
      StatusPublisher.OnNext(new DeviceStatus { DeviceState = DeviceState.Inactive });
  }

  public override async Task<bool> Activate()
  {
    if (Connection is null)
      throw new Exception("Cannot activate serial device before providing connection.");

    if (await Connection.ConnectionStatusUpdates.FirstAsync() == ConnectionStatus.Connected)
      return await Validate();

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

    return await Validate();
  }

  protected abstract Task<bool> Validate();
}
