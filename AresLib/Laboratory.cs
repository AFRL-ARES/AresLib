using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib.Device;
using DynamicData;
using DynamicData.Binding;

namespace AresLib
{
  // TODO? Proto messagify? figure it out later on, but keep it POD (maybe readonly observable transformation binding subscriptions)
  public class Laboratory
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

    internal ISourceCache<IDeviceCommandInterpreter<AresDevice>, string> AvailableDeviceCommandCompilerFactoriesSource { get; }
      = new SourceCache<IDeviceCommandInterpreter<AresDevice>, string>(deviceCommandCompilerFactory => deviceCommandCompilerFactory.Device.Name);

    public ReadOnlyObservableCollection<IDeviceCommandInterpreter<AresDevice>> AvailableDeviceCommandCompilerFactories { get; }


    // TODO: Planners? Analyzers? 
  }
}
