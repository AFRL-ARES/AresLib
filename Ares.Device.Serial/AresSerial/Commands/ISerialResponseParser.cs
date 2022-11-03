using System;
using System.Collections.Generic;

namespace Ares.Device.Serial.Commands;

internal interface ISerialResponseParser
{
  bool TryParseResponse(byte[] buffer, out SerialResponse? response, out ArraySegment<byte>? dataToRemove);
}
