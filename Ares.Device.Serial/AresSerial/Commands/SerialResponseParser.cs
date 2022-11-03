using System;
using System.Collections.Generic;

namespace Ares.Device.Serial.Commands;

public abstract class SerialResponseParser<T> : ISerialResponseParser where T : SerialResponse
{
  public bool TryParseResponse(byte[] buffer, out SerialResponse? response, out ArraySegment<byte>? dataToRemove)
  {
    var test = TryParseResponse(buffer, out T? typedResponse, out var toRemove);
    response = typedResponse;
    dataToRemove = toRemove;
    return test;
  }

  public abstract bool TryParseResponse(byte[] buffer, out T? response, out ArraySegment<byte>? dataToRemove);
}
