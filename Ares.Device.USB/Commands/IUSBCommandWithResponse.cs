using System;

namespace Ares.Device.USB.Commands;
public interface IUSBCommandWithResponse
{
    Guid Id { get; internal set; }

    string Response { get; internal set; }
}
