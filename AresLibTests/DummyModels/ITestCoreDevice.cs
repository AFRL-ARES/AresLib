using System;
using System.Threading.Tasks;
using AresDevicePluginBase;

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
