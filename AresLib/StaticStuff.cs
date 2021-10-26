using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Compilers;

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
