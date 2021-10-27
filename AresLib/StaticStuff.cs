﻿using Ares.Core;
using AresLib.Device;

namespace AresLib
{
  internal static class StaticStuff
  {
    public static string QualifyCommandName(IDeviceCommandInterpreter<AresDevice> interpreter, CommandMetadata commandMetadata)
    {
      return $"{interpreter.Device.Name}:{commandMetadata.Name}";
    }

    public static string QualifyCommandName(CommandMetadata commandMetadata)
    {
      return $"{commandMetadata.DeviceName}:{commandMetadata.Name}";
    }
  }
}
