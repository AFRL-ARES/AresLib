namespace Ares.Device.Serial.Commands;

internal interface ISerialCommandWithResponse
{
  ISerialResponseParser ResponseParser { get; }
}
