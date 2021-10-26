using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib;
using AresLib.Device;
using DynamicData;

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
