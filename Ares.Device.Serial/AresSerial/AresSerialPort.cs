using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Ares.Device.Serial.Simulation;

namespace Ares.Device.Serial
{
  public abstract class AresSerialPort : IAresSerialPort
  {

    protected AresSerialPort(SerialPortConnectionInfo connectionInfo)
    {
      ConnectionInfo = connectionInfo;
      DataBufferState = DataBufferStatePublisher.AsObservable();
      DataBufferState.Subscribe(ProcessBufferCore);
    }

    private void ProcessBufferCore(List<byte> currentData)
    {
      var preprocessedSize = currentData.Count;
      ProcessBuffer(ref currentData);
      var processedSize = currentData.Count;
      if (processedSize == preprocessedSize)
      {
        return;
      }
      DataBufferStatePublisher.OnNext(currentData);
    }

    public void AttemptOpen(string portName)
    {
      try
      {
        Open(portName);
        if (IsOpen)
        {
          Name = portName;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
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
      {
        return;
      }

      throw new InvalidOperationException(
        $"Successfully attempted to open port, but the port is not reporting as open");
    }
    protected void AddDataReceived(byte[] dataReceived)
    {
      var currentData = DataBufferState.Take(1).Wait();
      currentData.AddRange(dataReceived);
      DataBufferStatePublisher.OnNext(currentData);
    }

    public abstract void Disconnect();

    public abstract void SendOutboundMessage(byte[] input);
    protected abstract void ProcessBuffer(ref List<byte> currentData);
    protected abstract void Open(string portName);

    public virtual void Listen()
    {
    }

    public virtual void StopListening()
    {

    }

    public IObservable<List<byte>> DataBufferState { get; }
    public string? Name { get; private set; }
    public bool IsOpen { get; protected set; }
    protected SerialPortConnectionInfo ConnectionInfo { get; }
    protected ISubject<List<byte>> DataBufferStatePublisher { get; } = new BehaviorSubject<List<byte>>(new List<byte>());
  }
}
