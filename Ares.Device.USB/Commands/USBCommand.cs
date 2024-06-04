using System;

namespace Ares.Device.USB.Commands;
public class USBCommand
{
    public Guid Id { get; internal set; } = Guid.NewGuid();

    public string Name { get; internal set; } = "";
}
