using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace AresLib
{
  internal interface ICommandComposer<out DbTemplate> where DbTemplate : IMessage
  {
    Task Compose();
    DbTemplate Template { get; init; } 

    DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
