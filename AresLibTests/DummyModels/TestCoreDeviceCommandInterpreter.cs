using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Device;

namespace AresLibTests.DummyModels
{
  public class TestCoreDeviceCommandInterpreter : DeviceCommandInterpreter<TestCoreDevice, TestCoreDeviceCommand>
  {
    public TestCoreDeviceCommandInterpreter(TestCoreDevice testCoreDevice) : base(testCoreDevice)
    { }

    protected override void ParseAndPerformDeviceAction(TestCoreDeviceCommand deviceCommandEnum, CommandParameter[] commandParameters)
    {
      throw new NotImplementedException();
    }

    public override CommandMetadata[] CommandsToMetadatas()
    {
      throw new NotImplementedException();
    }
  }
}
