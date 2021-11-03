namespace AresSerial
{
  public abstract class SerialDevice<TConnection> : ISerialDevice<TConnection> where TConnection : SerialConnection, new()
  {
    protected TConnection Connection { get; set; }
  }
}
