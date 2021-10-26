using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.AresCoreDevice;
using AresLib.Compilers;
using DynamicData;

namespace AresLib
{
  // TODO? Proto messagify?
  internal class Laboratory
  {
    public Laboratory()
    {
      AvailableDeviceCommandCompilerFactoriesSource
        .Connect()
        .Bind(out var availableDeviceCommandCompilerFactories)
        .Subscribe();
      AvailableDeviceCommandCompilerFactories = availableDeviceCommandCompilerFactories;
    }

    public string Name { get; }

    internal ISourceCache<IDeviceCommandCompilerFactory<IAresDevice>, string> AvailableDeviceCommandCompilerFactoriesSource { get; }
      = new SourceCache<IDeviceCommandCompilerFactory<IAresDevice>, string>(deviceCommandCompilerFactory => deviceCommandCompilerFactory.Device.Name);

    public ReadOnlyObservableCollection<IDeviceCommandCompilerFactory<IAresDevice>> AvailableDeviceCommandCompilerFactories { get; }

    // TODO: Planners? Analyzers? 
  }
}
