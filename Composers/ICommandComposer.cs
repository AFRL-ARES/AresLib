﻿using Google.Protobuf;

namespace AresLib
{
  internal interface ICommandComposer<DbTemplate> where DbTemplate : IMessage
  {
    IBaseExecutable Compose();
    DbTemplate Template { get; init; }

    DeviceCommandCompilerRepoBridge DeviceCommandCompilerRepoBridge { get; init; }
  }
}
