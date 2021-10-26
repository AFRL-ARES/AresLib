using System.Collections.Generic;
using AresLib.Device;
using AresLib.Executors;
using Google.Protobuf;

namespace AresLib.Composers
{
  internal abstract class CommandComposer<DbTemplate, CoreExecutable> : ICommandComposer<DbTemplate, CoreExecutable> 
    where DbTemplate : IMessage
    where CoreExecutable : IBaseExecutor
  {
    public abstract CoreExecutable Compose();

    public DbTemplate Template { get; init; }

    public IDictionary<string, IDeviceCommandInterpreter<AresDevice>> CommandNamesToInterpreters { get; init; }
  }
}
