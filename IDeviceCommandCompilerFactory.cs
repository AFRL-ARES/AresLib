using Ares.Core;
using System;

namespace AresLib
{
  public interface IDeviceCommandCompilerFactory<QualifiedDevice> where QualifiedDevice : AresDevice
  {
    public QualifiedDevice Device { get; init; }

    public IDeviceCommandCompiler Create(CommandTemplate commandTemplate);
  }
}
