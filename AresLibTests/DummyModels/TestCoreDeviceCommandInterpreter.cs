using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.Device;
using UnitsNet;

namespace AresLibTests.DummyModels
{
  public class TestCoreDeviceCommandInterpreter : DeviceCommandInterpreter<TestCoreDevice, TestCoreDeviceCommand>
  {
    public TestCoreDeviceCommandInterpreter(TestCoreDevice testCoreDevice) : base(testCoreDevice)
    { }

    protected override void ParseAndPerformDeviceAction(
      TestCoreDeviceCommand deviceCommandEnum, CommandParameter[] commandParameters)
    {
      switch (deviceCommandEnum)
      {
        case TestCoreDeviceCommand.Wait:
          ParseAndPerformWait(commandParameters[0]);
          break;
      }
    }


    private void ParseAndPerformWait(CommandParameter durationParameter)
    {
      var duration = TimeSpan.FromSeconds(durationParameter.Value);
      Device.Wait(duration).Wait();
    }

    private CommandMetadata GenerateWaitCommandMetadata()
    {
      var waitCommandMetadata = new CommandMetadata();
      waitCommandMetadata.Name = $"{TestCoreDeviceCommand.Wait}";
      waitCommandMetadata.DeviceName = Device.Name;
      waitCommandMetadata.Description = "Simply waits for the specified duration";

      var durationParameterMetadata = new CommandParameterMetadata();
      durationParameterMetadata.Name = Duration.Info.Name;
      durationParameterMetadata.Constraints.Add(1);
      durationParameterMetadata.Constraints.Add(3);
      durationParameterMetadata.Unit = Duration.Info.BaseUnitInfo.Name;
      waitCommandMetadata.ParameterMetadatas.Add(durationParameterMetadata);


      return waitCommandMetadata;
    }

    public override CommandMetadata[] CommandsToMetadatas()
    {
      var commandMetadatas = new List<CommandMetadata>();

      var waitCommandMetadata = GenerateWaitCommandMetadata();
      commandMetadatas.Add(waitCommandMetadata);

      return commandMetadatas.ToArray();
    }


  }
}
