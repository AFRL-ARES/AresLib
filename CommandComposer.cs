using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace AresLib
{
  internal abstract class CommandComposer<DbTemplate> : ICommandComposer<DbTemplate> where DbTemplate : IMessage
  {
    public abstract Task Compose();

    public DbTemplate Template { get; init; }

    public DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
