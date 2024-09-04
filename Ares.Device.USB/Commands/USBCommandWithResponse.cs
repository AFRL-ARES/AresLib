using System;

namespace Ares.Device.USB.Commands;
public abstract class USBCommandWithResponse<T> : USBCommand
{
    public USBResponse? Response { get; set; }

    public Guid Guid { get; set; } = Guid.Empty;
}
