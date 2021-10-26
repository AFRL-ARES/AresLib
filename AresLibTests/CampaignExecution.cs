using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AresLibTests
{
  [TestClass]
  public class CampaignExecution
  {

    [TestMethod]
    public void Run()
    {
      Console.WriteLine("Yay, a test");
//      _coreCompFact = new CoreDeviceCommandCompilerFactory { Device = _coreDevice };
//      var factoryRepoBridge = new DeviceCommandCompilerFactoryRepoBridge();
//      factoryRepoBridge.Repo.AddOrUpdate(_coreCompFact);
//
//
//      var template = GenerateNewTemplate();
//      var composer = new CampaignComposer
//                     {
//                       Template = template,
//                       DeviceCommandCompilerFactoryRepoBridge = factoryRepoBridge
//                     };
//
//      var composition = composer.Compose();
//      Console.WriteLine("Hopefully going to wait for a key entry before executing the campaign");
//      Console.WriteLine("Going to call execution on the campaign, maybe.");
//      Console.ReadKey();
//      Task.Run(() => composition.Execute()).Wait();
    }


//
//    public static CampaignTemplate GenerateNewTemplate()
//    {
//      var wait1Command = WaitCommand(1);
//      var wait2Command = WaitCommand(2);
//      var wait3Command = WaitCommand(3);
//      var wait4Command = WaitCommand(4);
//      var wait5Command = WaitCommand(5);
//      var stepTemplate1 = new StepTemplate
//      {
//        IsParallel = false,
//        Name = "Derp1",
//        CommandTemplates =
//                            {
//                              wait3Command,
//                              wait1Command,
//                            }
//      };
//      var stepTemplate2 = new StepTemplate
//      {
//        IsParallel = false,
//        Name = "Derp2",
//        CommandTemplates =
//                            {
//                              wait2Command,
//                              wait1Command,
//                            }
//      };
//      var stepTemplate3 = new StepTemplate
//      {
//        IsParallel = false,
//        Name = "Derp3",
//        CommandTemplates =
//                            {
//                              wait4Command,
//                              wait2Command,
//                              wait1Command,
//                              wait3Command,
//                            }
//      };
//      var experimentTemplate = new ExperimentTemplate
//      {
//        StepTemplates =
//                                 {
//                                   stepTemplate1,
//                                   stepTemplate2,
//                                   stepTemplate3
//                                 }
//      };
//      var campaignTemplate = new CampaignTemplate
//      {
//        ExperimentTemplates =
//                               {
//                                 experimentTemplate
//                               }
//      };
//
//      return campaignTemplate;
//
//    }
//
//    public static CommandParameter DefaultWait(int seconds)
//    {
//      return new CommandParameter
//      {
//        Metadata = new CommandParameterMetadata
//        {
//          Name = "duration",
//          Unit = "seconds"
//        },
//        Value = seconds
//      };
//    }
//
//    public static CommandTemplate WaitCommand(int seconds)
//      =>
//        new CommandTemplate
//        {
//          Arguments =
//          {
//            DefaultWait(seconds)
//          },
//          Metadata =
//            new CommandMetadata
//            {
//              Name = $"{CoreDeviceCommandType.Wait}",
//              DeviceName = $"{_coreDevice.Name}"
//            }
//        };

  }
}
