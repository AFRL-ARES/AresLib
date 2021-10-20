using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AresLib.AresCoreDevice.Commands;

namespace AresLib.AresCoreDevice
{
  internal class CoreDevice : AresDevice<CoreDeviceCommand>, ICoreDevice
  {
    public void Wait(TimeSpan duration)
    {
      Thread.Sleep(duration);
    }

    public ReadOnlyObservableCollection<CommandMetadata> AvailableCommands { get; }

    public Guid Id { get; }

  }
}
