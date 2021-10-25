using AresLib.Compilers;
using Google.Protobuf;

namespace AresLib.Composers
{
  internal abstract class CommandComposer<DbTemplate, CoreExecutable> : ICommandComposer<DbTemplate, CoreExecutable> 
    where DbTemplate : IMessage
    where CoreExecutable : IBaseExecutor
  {
    public abstract CoreExecutable Compose();

    public DbTemplate Template { get; init; }

    public DeviceCommandCompilerFactoryRepoBridge DeviceCommandCompilerFactoryRepoBridge { get; init; }
  }
}
