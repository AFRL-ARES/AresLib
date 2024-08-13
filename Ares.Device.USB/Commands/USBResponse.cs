using System;

namespace Ares.Device.USB.Commands;
public abstract class USBResponse
{
    public Guid RequestId { get; internal set; }
}
