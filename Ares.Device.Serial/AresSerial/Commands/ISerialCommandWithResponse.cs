using System;
using System.Collections.Generic;

namespace Ares.Device.Serial.Commands;

internal interface ISerialCommandWithResponse
{
  bool TryParse(IEnumerable<byte> buffer, out ISerialResponse? response, out ArraySegment<byte>? dataToRemove);
}
