using AresLib;
using DynamicData;
using System;
using System.Linq;

namespace AresLibTests.DummyModels
{
  public class TestLaboratoryManager : LaboratoryManager
  {
    public TestLaboratoryManager()
    {
      var devices =
        Enumerable.Range(1, 3)
                  .Select(address => new TestCoreDevice(address))
                  .ToArray();
      var coreDeviceInterpreters =
        devices
          .Select(device => new TestCoreDeviceCommandInterpreter(device))
          .ToArray();

      DeviceCommandInterpretersSource.AddOrUpdate(coreDeviceInterpreters);
    }

    protected override Laboratory BuildLab()
    {
      DeviceCommandInterpretersSource
        .Connect()
        .Bind(out var managedDeviceInterpreters)
        .Subscribe();

      var testLab = new Laboratory("TestLaboratory", managedDeviceInterpreters);

      return testLab;
    }
  }
}
