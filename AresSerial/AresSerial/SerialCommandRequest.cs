namespace AresSerial
{
  public abstract class SerialCommandRequest
  {
    public SerialCommandRequest(bool expectsResponse)
    {
      ExpectsResponse = expectsResponse;
    }

    public abstract string Serialize();
    public bool ExpectsResponse { get; }
  }
}
