namespace Ares.Device.Serial.Commands;

public abstract class SerialCommand
{
  internal byte[] SerializedData => Serialize();

  protected abstract byte[] Serialize();
}
