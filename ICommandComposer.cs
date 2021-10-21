using Google.Protobuf;
using System.Threading.Tasks;

namespace AresLib
{
  internal interface ICommandComposer<DbTemplate> where DbTemplate : IMessage
  {
    Task Compose();
    DbTemplate Template { get; init; }

    DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
