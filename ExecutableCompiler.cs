using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLib
{
  internal abstract class ExecutableCompiler : IExecutableCompiler
  {
    public abstract Task GenerateExecutable();

    public DeviceTranslatorRepoBridge DeviceTranslatorRepoBridge { get; init }
  }
}
