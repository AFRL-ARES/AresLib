using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AresLib;
using AresLib.Device;

namespace AresLibTests.DummyModels
{
  public interface ITestCoreDevice : IAresDevice
  {
    Task Wait(TimeSpan duration);
    void Derp();
    int Test();

    int Address { get; }
  }
}
