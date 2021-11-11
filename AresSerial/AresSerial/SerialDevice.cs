﻿using AresDevicePluginBase;
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
          .Where(response => response.SourceRequest == validationRequest)
          .Take(1)
          .ToTask();
      var timeout = TimeSpan.FromSeconds(5);
      var responseTimeout = Task.Delay(timeout);
      Connection.SendCommand(validationRequest);
      var fasterTask = await Task.WhenAny(responseWaiter, responseTimeout);
      if (fasterTask == responseTimeout)
      {
        throw new TimeoutException($"Did not receive an expected valid response within {timeout}");
      }

      listenerStatus = await Connection.ListenerStatusUpdates.Take(1);

      return listenerStatus != ListenerStatus.Paused;

      // if (connectionStatus == ConnectionStatus.InvalidResponse)
      // {
      //   throw new Exception($"Received an invalid response from device");
      // }
      //
      // if (connectionStatus != ConnectionStatus.ValidResponse)
      // {
      //   throw new Exception($"Not sure if it's supposed to possible to reach this exception. What still needs to be handled?");
      // }
    }

    public abstract TConnection EstablishConnection();

    protected TConnection Connection { get; set; }
  }
}
