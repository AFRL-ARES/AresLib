using System;
using System.Collections.Generic;
using System.Text;

namespace AresSerial
{
  public abstract class SerialCommandRequestWithResponse<T> : SerialCommandRequest where T : SerialCommandResponse
  {
    public abstract T DeserializeResponse(string response);
  }
}
