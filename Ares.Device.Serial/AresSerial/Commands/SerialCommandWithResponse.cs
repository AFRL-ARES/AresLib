namespace Ares.Device.Serial.Commands;

public abstract class SerialCommandWithResponse<T> : SerialCommand, ISerialCommandWithResponse where T : ISerialResponse
{

  public SerialCommandWithResponse(SerialResponseParser<T> parser)
  {
    Parser = parser;
  }

  internal SerialResponseParser<T> Parser { get; }

  ISerialResponseParser ISerialCommandWithResponse.ResponseParser => Parser;
}
