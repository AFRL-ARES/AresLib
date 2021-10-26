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
      TestCoreDeviceCommand deviceCommandEnum, Parameter[] commandParameters)
    {
      switch (deviceCommandEnum)
      {
        case TestCoreDeviceCommand.Wait:
          ParseAndPerformWait(commandParameters[0]);
          break;
      }
    }


    private void ParseAndPerformWait(Parameter durationParameter)
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

      var durationParameterMetadata = new ParameterMetadata();
      durationParameterMetadata.Name = Duration.Info.Name;
      var durationParameterLimits = new Limits();
      durationParameterLimits.Minimum = 1;
      durationParameterLimits.Maximum = 3;
      durationParameterMetadata.Constraints.Add(durationParameterLimits);
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
