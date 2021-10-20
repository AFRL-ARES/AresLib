using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal class CommandCompiler : ExecutableCompiler, ICommandCompiler
  {
    public override Task GenerateExecutable()
    {
      var deviceName = Command.Metadata.DeviceName;
      var translatorLookup = DeviceTranslatorRepoBridge.Repo.Lookup(deviceName);
      var translator = translatorLookup.Value;
      // If translator is null/exception, the current system does not have the required device

      var deviceCommand = translator.GenerateDeviceCommand(Command);
      return deviceCommand.Execution;
    }

    public AresCommand Command { get; init; }
  }
}
