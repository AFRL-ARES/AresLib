namespace Ares.Device.Serial;

public abstract class SerialCommandRequestWithResponse<T> : SerialCommandRequest where T : SerialCommandResponse
{
  public abstract T DeserializeResponse(string response);
}
