using System.Linq;
using AresDeviceBase;

namespace AresLibTests.DummyModels;

public class TestLaboratoryManager : LaboratoryManager
{
  public TestLaboratoryManager() : base("TestLaboratory")
  {
    foreach (var interpreter in GenerateDeviceCommandInterpreters())
      RegisterDeviceInterpreter(interpreter).Wait();
  }

  protected override IDeviceCommandInterpreter<AresDevice>[] GenerateDeviceCommandInterpreters()
  {
    var devices =
      Enumerable.Range(1, 3)
        .Select(address => new TestCoreDevice(address))
        .ToArray();

    var coreDeviceInterpreters =
      devices
        .Select(device => new TestCoreDeviceCommandInterpreter(device))
        .ToArray();

    return coreDeviceInterpreters;
  }
}
