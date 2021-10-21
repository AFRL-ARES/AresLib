using Google.Protobuf;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class CommandComposer<DbTemplate> : ICommandComposer<DbTemplate> where DbTemplate : IMessage
  {
    public abstract Task Compose();

    public DbTemplate Template { get; init; }

    public DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
