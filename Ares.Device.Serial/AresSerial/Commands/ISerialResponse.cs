using System;

namespace Ares.Device.Serial.Commands;

public interface ISerialResponse
{
  Guid RequestId { get; internal set; }
}
