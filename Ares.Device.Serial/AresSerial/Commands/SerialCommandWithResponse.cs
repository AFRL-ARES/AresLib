using System;
using System.Collections.Generic;

namespace Ares.Device.Serial.Commands;

public abstract class SerialCommandWithResponse<T> : SerialCommand, ISerialCommandWithResponse where T : ISerialResponse
{
  public bool Stream { get; set; }

  public bool TryParse(IEnumerable<byte> buffer, out ISerialResponse? response, out ArraySegment<byte>? dataToRemove)
  {
    var test = TryParse(buffer, out T? typedResponse, out var toRemove);
    response = typedResponse;
    dataToRemove = toRemove;
    return test;
  }

  public abstract bool TryParse(IEnumerable<byte> buffer, out T? response, out ArraySegment<byte>? dataToRemove);
}
