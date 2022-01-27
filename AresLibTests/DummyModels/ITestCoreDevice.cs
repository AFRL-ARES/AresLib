using System;
using System.Threading.Tasks;
using AresDeviceBase;

namespace AresLibTests.DummyModels;

public interface ITestCoreDevice : IAresDevice
{

  int Address { get; }
  Task Wait(TimeSpan duration);
  void Derp();
  int Test();
}
