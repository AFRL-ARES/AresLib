using System.Collections.Generic;
using AresLib.Compilers;
using Google.Protobuf;

namespace AresLib.Composers
{
  internal interface ICommandComposer<DbTemplate, out CoreExecutable> 
    where DbTemplate : IMessage
    where CoreExecutable : IBaseExecutor
  {
    CoreExecutable Compose();
    DbTemplate Template { get; init; }

    IDictionary<string, IDeviceCommandInterpreter<AresDevice>> CommandNamesToInterpreters { get; init; }
  }
}
