using AresDevicePluginBase;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
      Connection = EstablishConnection();
      var connectionStatus = await Connection.StatusUpdates.Take(1);
      if (connectionStatus != ConnectionResult.Unattempted)
      {
        throw new Exception("This really shouldn't ever be thrown, probably only used for testing purposes");
      }

      Connection.Connect();
      connectionStatus = await Connection.StatusUpdates.Take(1);
      if (connectionStatus != ConnectionResult.Connected)
      {
        throw new Exception($"Failing to connect should result in {ConnectionResult.FailedConnection} and throw before this statement, or be {ConnectionResult.Connected}. Result is {connectionStatus}");
      }
      Connection.Listen();
      await Task.Delay(1000);
      connectionStatus = await Connection.StatusUpdates.Take(1);
      if (connectionStatus != ConnectionResult.Listening)
      {
        return false;
      }

      var responseWaiter =
        Connection
          .StatusUpdates
          .TakeWhile(status => status < ConnectionResult.ListenerPaused)
          .ToTask();
      var validationRequest = Connection.GenerateValidationRequest();
      var timeout = TimeSpan.FromSeconds(3);
      var responseTimeout = Task.Delay(timeout);
      Connection.SendCommand(validationRequest);
      var fasterTask = await Task.WhenAny(responseWaiter, responseTimeout);
      if (fasterTask == responseTimeout)
      {
        throw new TimeoutException($"Did not receive an expected valid response within {timeout}");
      }

      connectionStatus = await Connection.StatusUpdates.Take(1);
      if (connectionStatus == ConnectionResult.ListenerPaused)
      {
        return false;
      }

      if (connectionStatus == ConnectionResult.InvalidResponse)
      {
        throw new Exception($"Received an invalid response from device");
      }

      if (connectionStatus != ConnectionResult.ValidResponse)
      {
        throw new Exception($"Not sure if it's supposed to possible to reach this exception. What still needs to be handled?");
      }

      return true;
    }

    public abstract TConnection EstablishConnection();

    protected TConnection Connection { get; set; }
  }
}
