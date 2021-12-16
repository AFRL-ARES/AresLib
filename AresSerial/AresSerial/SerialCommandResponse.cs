using System;
namespace AresSerial
{
  public abstract class SerialCommandResponse
  {
    public SerialCommandResponse(string message, SerialCommandRequest sourceRequest)
    {
      Message = message;
      SourceRequest = sourceRequest;
    }

    public string Message { get; }
    public SerialCommandRequest SourceRequest { get; }
  }
}
