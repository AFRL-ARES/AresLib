using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ares.Core;
using AresLib.AresCoreDevice;
using Google.Protobuf;

namespace AresLib
{
//  internal class QualifiedCompiler : Compiler<CommandTemplate>
//  {
//    public ICoreDevice Device { get; }
//    public override Task Compile()
//    {
//      if (Template.Metadata.Name == "WAIT")
//      {
//        var durationVal = Template.Arguments.First()
//                                  .Value;
//        var duration = TimeSpan.FromSeconds(durationVal);
//        var qualifiedCommand = new Task(() => Device.Wait(duration));
//        return qualifiedCommand;
//      }
//
//      return Task.FromException(new Exception("Failed to create a task"));
//    }
//  }
  internal abstract class Compiler<T> : ICompiler<T> where T : IMessage
  {
    public abstract Task Compile();
  }
}
