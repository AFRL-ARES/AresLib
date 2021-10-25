using Google.Protobuf;

namespace AresLib
{
  internal abstract class CommandComposer<DbTemplate> : ICommandComposer<DbTemplate> where DbTemplate : IMessage
  {
    public abstract IBaseExecutable Compose();

    public DbTemplate Template { get; init; }

    public DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
